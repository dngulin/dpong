using System;
using System.Collections.Generic;
using DPong.Level.Data;
using DPong.Level.Model;
using DPong.Level.Networking;
using DPong.Level.State;
using DPong.Level.UI;
using DPong.Level.View;
using DPong.UI;
using NGIS.Message.Client;
using NGIS.Message.Server;
using PGM.Random;
using PGM.ScaledNum;

namespace DPong.Level {
  public class NetworkLevel : ILevelUIListener, IDisposable {
    private const uint InputDelay = 2;
    private readonly IInputSource _inputSrc;
    private bool _paused;

    private readonly Side _side;

    private readonly LevelModel _model;
    private readonly LevelView _view;
    private readonly LevelUI _ui;

    private readonly FrameTimer _frameTimer;
    private readonly StateBuffer _stateBuffer;
    private readonly InputBuffer _inputBuffer;

    private readonly Queue<ClientMsgInputs> _inputSendQueue;

    private ProcessingState _processingState;

    private uint _frame; // Simulated frame index
    private uint _simulationCounter;

    private readonly ILevelExitListener _exitListener;

    public (uint, uint) SimulationStats => (_frame, _simulationCounter);

    public NetworkLevel(IInputSource inputSrc, UISystem uiSystem, ILevelExitListener exitListener, ServerMsgStart msgStart) {
      _inputSrc = inputSrc;
      _exitListener = exitListener;
      _side = (Side) msgStart.YourIndex;

      // Buffers layout & usage
      // -----------------------------------------
      // Frame:  n-3  n-2  n-1   n   n+1  n+2  n+3
      // State:  [ ]  [ ]  [ ]  [^]
      // Input:  [ ]  [ ]  [ ]  [^]  [ ]  [ ]  [ ]
      // -----------------------------------------
      // State[0] = f(settings)
      // State[n+1] = f(State[n], Input[n])

      var bufferRange = msgStart.TicksPerSecond / 2 + 1;
      _stateBuffer = new StateBuffer(bufferRange);
      _inputBuffer = new InputBuffer(bufferRange * 2 - 1);

      _inputSendQueue = new Queue<ClientMsgInputs>(bufferRange);

      for (uint frame = 0; frame < InputDelay; frame++)
        _inputBuffer[frame].Approved = true;

      var left = new PlayerInfo(msgStart.Players[0], msgStart.YourIndex == 0 ? PlayerType.Local : PlayerType.Remote);
      var right = new PlayerInfo(msgStart.Players[1], msgStart.YourIndex == 1 ? PlayerType.Local : PlayerType.Remote);

      long tickDuration = SnMath.One / msgStart.TicksPerSecond;
      var randomState = Pcg.CreateState(new Random(msgStart.Seed));
      var simSettings = new SimulationSettings(tickDuration, randomState);

      var settings = new LevelSettings(left, right, simSettings);

      _model = new LevelModel(settings);
      _stateBuffer[0] = _model.CreateInitialState();

      _view = new LevelView(_stateBuffer[0], settings);
      _ui = new LevelUI(uiSystem, this);

      _processingState = ProcessingState.Active;

      _frameTimer = new FrameTimer(tickDuration);
    }

    public bool HandleReceivedInputs(Queue<ServerMsgInput> remoteInputs) {
      while (remoteInputs.Count > 0) {
        if (!HandleReceivedInput(remoteInputs.Dequeue()))
          return false;
      }

      return true;
    }

    private bool HandleReceivedInput(ServerMsgInput msgInput) {
      if (msgInput.PlayerIndex == (byte) _side) {
        // TODO:  _log.Error("My side inputs received");
        return false;
      }

      var (min, max) = _inputBuffer.GetFramesRange(_frame);
      if (msgInput.Frame < min || msgInput.Frame > max || msgInput.Frame < InputDelay) {
        // TODO: _log.Error($"Failed to write frame input {msgInput.Frame} ({min}, {max})");
        return false;
      }

      var remoteFrame = msgInput.Frame - InputDelay;
      if (remoteFrame > _frame)
        _frameTimer.SyncCurrentFrame(remoteFrame);

      var receivedKeys = (Keys) msgInput.InputMask;

      for (var frame = msgInput.Frame; frame <= max; frame++) {
        ref var input = ref _inputBuffer[frame];

        if (input.Approved) {
          // TODO: _log.Error("Try to rewrite approved input");
          return false;
        }

        ref var storedKeys = ref msgInput.PlayerIndex == 0 ? ref input.Left : ref input.Right;

        if (frame == msgInput.Frame) {
          input.Approved = true;
          if (frame < _frame)
            input.MisPredicted = receivedKeys != storedKeys; // Was simulated with wrong value
        }

        storedKeys = receivedKeys; // Primitive input state prediction
      }

      return true;
    }

    public (Queue<ClientMsgInputs>, ClientMsgFinished?) Tick() {
      switch (_processingState) {
        case ProcessingState.Inactive:
        case ProcessingState.FinishedByApprovedInput:
          return (_inputSendQueue, null);

        case ProcessingState.Active:
          _processingState = Simulate();
          break;

        case ProcessingState.FinishedByPredictedInput:
          _processingState = _inputBuffer[_frame - 1].Approved ? ProcessingState.FinishedByApprovedInput : Simulate();
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }

      ClientMsgFinished? finishMsg = null;
      if (_processingState == ProcessingState.FinishedByApprovedInput)
        finishMsg = new ClientMsgFinished(_frame, _stateBuffer[_frame].CalculateHash());

      if (_frame > 0)
        _view.UpdateState(_stateBuffer[_frame - 1], _stateBuffer[_frame], _frameTimer.GetBlendingFactor(_frame));

      return (_inputSendQueue, finishMsg);
    }

    private ProcessingState Simulate() {
      var resimulationResult = ProcessMisPredictedStates();
      if (resimulationResult != ProcessingState.Active)
        return resimulationResult;

      return ProcessFutureStates();
    }

    private ProcessingState ProcessMisPredictedStates() {
      var simulationState = _processingState;

      for (var frame = _inputBuffer.GetFirstMisPredictedFrame(_frame); frame < _frame; frame++) {
        var finished = SimulateNextState(frame);

        ref var input = ref _inputBuffer[frame];
        if (input.Approved)
          input.MisPredicted = false;

        if (finished) {
          _frame = frame + 1;
          return input.Approved ? ProcessingState.FinishedByApprovedInput : ProcessingState.FinishedByPredictedInput;
        }

        simulationState = ProcessingState.Active;
      }

      return simulationState;
    }

    private ProcessingState ProcessFutureStates() {
      var targetFrame = Math.Min(_frameTimer.Current, _inputBuffer.GetMaxReachableFrame(_frame));

      for (; _frame < targetFrame;) {
        PushLocalInputs();
        var finished = SimulateNextState(_frame++);
        _inputBuffer.HandleFrameIncremented(_frame);

        if (finished)
          return _inputBuffer[_frame - 1].Approved ?
            ProcessingState.FinishedByApprovedInput :
            ProcessingState.FinishedByPredictedInput;
      }

      return ProcessingState.Active;
    }

    private bool SimulateNextState(uint frame) {
      var state = _stateBuffer[frame];
      var input = _inputBuffer[frame];

      var finished = _model.Tick(ref state, input.Left, input.Right);
      _stateBuffer[frame + 1] = state;
      _simulationCounter++;

      return finished;
    }

    private void PushLocalInputs() {
      var frame = _frame + InputDelay;
      var keys = _paused ? Keys.None : _inputSrc.GetKeys();

      (_side == Side.Left ? ref _inputBuffer[frame].Left : ref _inputBuffer[frame].Right) = keys;
      _inputSendQueue.Enqueue(new ClientMsgInputs(frame, (ulong) keys));
    }

    public void Dispose() => _view?.Dispose();

    void ILevelUIListener.PauseCLicked() => _paused = true;
    void ILevelUIListener.ResumeCLicked() => _paused = false;
    void ILevelUIListener.ExitCLicked() => _exitListener.Exit();
  }
}