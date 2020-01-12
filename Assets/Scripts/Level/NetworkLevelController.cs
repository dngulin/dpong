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

namespace DPong.Level {
  public class NetworkLevelController : IDisposable {
    private const uint InputDelay = 2;
    private readonly ILocalInputSource _localInputSource;

    private readonly Side _side;
    private readonly long _tickDuration;

    private readonly LevelModel _model;
    private readonly LevelView _view;

    private SimulationState _simulationState;
    private uint _simulationCounter;

    private uint _frame;
    private readonly StateBuffer _states;

    private readonly Stopwatch _frameTimer = new Stopwatch();
    private uint _frameTimerOffset;

    private readonly InputBuffer _inputs;
    private readonly Queue<ClientMsgInputs> _inputSendQueue;

    public (uint, uint) SimulationStats => (_frame, _simulationCounter);

    public NetworkLevelController(ILocalInputSource localInputSource, ServerMsgStart msgStart) {
      _localInputSource = localInputSource;

      _side = (Side) msgStart.YourIndex;
      _tickDuration = 1000 / msgStart.TicksPerSecond;

      var stateCount = msgStart.TicksPerSecond / 2 + 1;
      _inputSendQueue = new Queue<ClientMsgInputs>(stateCount);

      _states = new StateBuffer(stateCount);
      _inputs = new InputBuffer(stateCount * 2 - 1);

      for (uint frame = 0; frame < InputDelay; frame++)
        _inputs[frame].Approved = true;

      var left = new PlayerInfo(msgStart.Players[0], msgStart.YourIndex == 0 ? PlayerType.Local : PlayerType.Remote);
      var right = new PlayerInfo(msgStart.Players[1], msgStart.YourIndex == 1 ? PlayerType.Local : PlayerType.Remote);

      var randomState = Pcg.CreateState(new Random(msgStart.Seed));
      var simSettings = new SimulationSettings(_tickDuration, randomState);

      var settings = new LevelSettings(left, right, simSettings);

      _model = new LevelModel(settings);
      _states[0] = _model.CreateInitialState();

      _view = new LevelView(_states[0], settings);
      _simulationState = SimulationState.Active;

      _frameTimer.Start();
    }

    public void InputReceived(ServerMsgInput msgInput) {
      if (msgInput.PlayerIndex == (byte) _side)
        throw new Exception("My side inputs received");

      var (min, max) = _inputs.GetWindow(_frame);
      if (msgInput.Frame < min || msgInput.Frame > max || msgInput.Frame < InputDelay)
        throw new Exception($"Failed to write frame input {msgInput.Frame} ({min}, {max})");

      var remoteFrame = msgInput.Frame - InputDelay;
      if (remoteFrame > _frame) {
        _frameTimer.Restart();
        _frameTimerOffset = remoteFrame;
      }

      var msgKeys = (Keys) msgInput.InputMask;

      for (var frame = msgInput.Frame; frame <= max; frame++) {
        ref var input = ref _inputs[frame];

        if (input.Approved) throw new Exception("Try to rewrite approved input");

        ref var enemySideKeys = ref msgInput.PlayerIndex == 0 ? ref input.Left : ref input.Right;

        if (frame == msgInput.Frame) {
          input.Approved = true;
          if (frame < _frame)
            input.MisPredicted = msgKeys != enemySideKeys; // Was simulated with wrong value
        }

        enemySideKeys = msgKeys; // Primitive input state prediction
      }
    }

    public (Queue<ClientMsgInputs>, ClientMsgFinished?) Process() {
      var simulationCounter = _simulationCounter;
      ClientMsgFinished? finishMsg = null;

      switch (_simulationState) {
        case SimulationState.Inactive:
        case SimulationState.Finished:
          return (_inputSendQueue, null);

        case SimulationState.Active:
          _simulationState = Simulate();
          break;

        case SimulationState.PreFinished:
          _simulationState = _inputs[_frame - 1].Approved ? SimulationState.Finished : Simulate();
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }

      if (_simulationState == SimulationState.Finished)
        finishMsg = new ClientMsgFinished(_frame, _states[_frame].CalculateHash());

      if (simulationCounter != _simulationCounter)
        _view.StateContainer.SetPreviousAndCurrentStates(_states[_frame - 1], _states[_frame]);

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

      for (var frame = GetFirstMisPredictedFrame(); frame < _frame; frame++) {
        var finished = SimulateNextState(frame);

        ref var input = ref _inputs[frame];
        if (input.Approved)
          input.MisPredicted = false;

        if (finished) {
          _frame = frame + 1;
          return input.Approved ? SimulationState.Finished : SimulationState.PreFinished;
        }

        simulationState = SimulationState.Active;
      }

      return simulationState;
    }

    private SimulationState ProcessFutureStates() {
      var targetFrame = GetTargetSimulationFrame();
      for (; _frame < targetFrame;) {
        PushLocalInputs(_side == Side.Left ? _localInputSource.GetLeft() : _localInputSource.GetRight());
        var finished = SimulateNextState(_frame++);
        _inputs.HandleFrameIncremented(_frame);

        if (finished)
          return _inputs[_frame - 1].Approved ? SimulationState.Finished : SimulationState.PreFinished;
      }

      return SimulationState.Active;
    }

    private uint GetTargetSimulationFrame() {
      var maxReachableFrame = Math.Max(_frame + _inputs.CountApproved(), _states.Count / 2);
      var timeBasedFrame = _frameTimerOffset + (uint) (_frameTimer.ElapsedMilliseconds / _tickDuration);

      return Math.Min(timeBasedFrame, maxReachableFrame);
    }

    private uint GetFirstMisPredictedFrame() {
      var (min, _) = _inputs.GetWindow(_frame);
      for (var frame = min; frame < _frame; frame++) {
        if (_inputs[frame].MisPredicted)
          return frame;
      }
      return _frame;
    }

    private bool SimulateNextState(uint frame) {
      var state = _states[frame];
      var input = _inputs[frame];

      var finished = _model.Tick(ref state, input.Left, input.Right);
      _states[frame + 1] = state;
      _simulationCounter++;

      return finished;
    }

    private void PushLocalInputs(Keys keys) {
      var frame = _frame + InputDelay;
      _inputSendQueue.Enqueue(new ClientMsgInputs(frame, (ulong) keys));
      (_side == Side.Left ? ref _inputs[frame].Left : ref _inputs[frame].Right) = keys;
    }

    public void Dispose() => _view?.Dispose();
  }
}