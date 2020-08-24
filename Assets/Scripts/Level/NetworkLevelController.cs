using System;
using System.Collections.Generic;
using System.Diagnostics;
using DPong.Level.Data;
using DPong.Level.Model;
using DPong.Level.Networking;
using DPong.Level.State;
using DPong.Level.View;
using NGIS.Message.Client;
using NGIS.Message.Server;
using PGM.Random;
using PGM.ScaledNum;

namespace DPong.Level {
  public class NetworkLevelController : IDisposable {
    private const uint InputDelay = 2;
    private readonly IInputSource _inputSrc;

    private readonly Side _side;
    private readonly long _tickDuration;

    private readonly LevelModel _model;
    private readonly LevelView _view;

    private SimulationState _simulationState;
    private uint _simulationCounter;

    private uint _frame;
    private readonly StateBuffer _stateBuffer;

    private readonly Stopwatch _frameTimer = new Stopwatch();
    private uint _frameTimerOffset;

    private readonly InputBuffer _inputBuffer;
    private readonly Queue<ClientMsgInputs> _inputSendQueue;

    public (uint, uint) SimulationStats => (_frame, _simulationCounter);

    public NetworkLevelController(IInputSource inputSrc, ServerMsgStart msgStart) {
      _inputSrc = inputSrc;

      _side = (Side) msgStart.YourIndex;
      _tickDuration = SnMath.One / msgStart.TicksPerSecond;

      var stateCount = msgStart.TicksPerSecond / 2 + 1;
      _inputSendQueue = new Queue<ClientMsgInputs>(stateCount);

      _stateBuffer = new StateBuffer(stateCount);
      _inputBuffer = new InputBuffer(stateCount * 2 - 1);

      for (uint frame = 0; frame < InputDelay; frame++)
        _inputBuffer[frame].Approved = true;

      var left = new PlayerInfo(msgStart.Players[0], msgStart.YourIndex == 0 ? PlayerType.Local : PlayerType.Remote);
      var right = new PlayerInfo(msgStart.Players[1], msgStart.YourIndex == 1 ? PlayerType.Local : PlayerType.Remote);

      var randomState = Pcg.CreateState(new Random(msgStart.Seed));
      var simSettings = new SimulationSettings(_tickDuration, randomState);

      var settings = new LevelSettings(left, right, simSettings);

      _model = new LevelModel(settings);
      _stateBuffer[0] = _model.CreateInitialState();

      _view = new LevelView(_stateBuffer[0], settings);
      _simulationState = SimulationState.Active;

      _frameTimer.Start();
    }

    public void InputReceived(ServerMsgInput msgInput) {
      if (msgInput.PlayerIndex == (byte) _side)
        throw new Exception("My side inputs received");

      var (min, max) = _inputBuffer.GetReachableFramesRange(_frame);
      if (msgInput.Frame < min || msgInput.Frame > max || msgInput.Frame < InputDelay)
        throw new Exception($"Failed to write frame input {msgInput.Frame} ({min}, {max})");

      var remoteFrame = msgInput.Frame - InputDelay;
      if (remoteFrame > _frame) {
        _frameTimer.Restart();
        _frameTimerOffset = remoteFrame;
      }

      var receivedKeys = (Keys) msgInput.InputMask;

      for (var frame = msgInput.Frame; frame <= max; frame++) {
        ref var input = ref _inputBuffer[frame];

        if (input.Approved) throw new Exception("Try to rewrite approved input");

        ref var storedKeys = ref msgInput.PlayerIndex == 0 ? ref input.Left : ref input.Right;

        if (frame == msgInput.Frame) {
          input.Approved = true;
          if (frame < _frame)
            input.MisPredicted = receivedKeys != storedKeys; // Was simulated with wrong value
        }

        storedKeys = receivedKeys; // Primitive input state prediction
      }
    }

    public (Queue<ClientMsgInputs>, ClientMsgFinished?) Process() {
      var simulationCounter = _simulationCounter;
      ClientMsgFinished? finishMsg = null;

      switch (_simulationState) {
        case SimulationState.Inactive:
        case SimulationState.FinishedByApprovedInput:
          return (_inputSendQueue, null);

        case SimulationState.Active:
          _simulationState = Simulate();
          break;

        case SimulationState.FinishedByPredictedInput:
          _simulationState = _inputBuffer[_frame - 1].Approved ? SimulationState.FinishedByApprovedInput : Simulate();
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }

      if (_simulationState == SimulationState.FinishedByApprovedInput)
        finishMsg = new ClientMsgFinished(_frame, _stateBuffer[_frame].CalculateHash());

      if (simulationCounter != _simulationCounter)
        _view.StateContainer.SetPreviousAndCurrentStates(_stateBuffer[_frame - 1], _stateBuffer[_frame]);

      return (_inputSendQueue, finishMsg);
    }

    private SimulationState Simulate() {
      var resimulationResult = ProcessMisPredictedStates();
      if (resimulationResult != SimulationState.Active)
        return resimulationResult;

      return ProcessFutureStates();
    }

    private SimulationState ProcessMisPredictedStates() {
      var simulationState = _simulationState;

      for (var frame = _inputBuffer.GetFirstMisPredictedFrame(_frame); frame < _frame; frame++) {
        var finished = SimulateNextState(frame);

        ref var input = ref _inputBuffer[frame];
        if (input.Approved)
          input.MisPredicted = false;

        if (finished) {
          _frame = frame + 1;
          return input.Approved ? SimulationState.FinishedByApprovedInput : SimulationState.FinishedByPredictedInput;
        }

        simulationState = SimulationState.Active;
      }

      return simulationState;
    }

    private SimulationState ProcessFutureStates() {
      var targetFrame = GetTargetSimulationFrame();
      for (; _frame < targetFrame;) {
        PushLocalInputs();
        var finished = SimulateNextState(_frame++);
        _inputBuffer.HandleFrameIncremented(_frame);

        if (finished)
          return _inputBuffer[_frame - 1].Approved ?
            SimulationState.FinishedByApprovedInput :
            SimulationState.FinishedByPredictedInput;
      }

      return SimulationState.Active;
    }

    private uint GetTargetSimulationFrame() {
      var maxReachableFrame = Math.Max(_frame + _inputBuffer.CountApproved(), _stateBuffer.Count - 1);
      var timeBasedFrame = _frameTimerOffset + (uint) (_frameTimer.ElapsedMilliseconds / _tickDuration);

      return Math.Min(timeBasedFrame, maxReachableFrame);
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
      var keys = _inputSrc.GetKeys();

      (_side == Side.Left ? ref _inputBuffer[frame].Left : ref _inputBuffer[frame].Right) = keys;
      _inputSendQueue.Enqueue(new ClientMsgInputs(frame, (ulong) keys));
    }

    public void Dispose() => _view?.Dispose();
  }
}