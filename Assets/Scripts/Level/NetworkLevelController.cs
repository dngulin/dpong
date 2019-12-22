using System;
using System.Collections.Generic;
using System.Diagnostics;
using DPong.Level.Data;
using DPong.Level.Model;
using DPong.Level.State;
using DPong.Level.View;
using NGIS.Message.Client;
using NGIS.Message.Server;
using NGIS.Session.Client;
using PGM.Random;
using PGM.ScaledNum;

namespace DPong.Level {
  public class NetworkLevelController : IGameClient, IDisposable {
    private const uint InputDelay = 2;
    private readonly ILocalInputSource _localInputSource;

    private Side _side;
    private long _tickDuration;

    private LevelModel _model;
    private LevelView _view;

    private Stopwatch _playingTime;
    private uint _frame;
    private LevelState[] _states;

    private bool _finished;

    private struct NetworkInputs {
      public Keys Left;
      public Keys Right;
      public bool Approved;
      public bool MisPredicted;
    }

    private NetworkInputs[] _inputs;

    private readonly Queue<ClientMsgInputs> _inputSendQueue = new Queue<ClientMsgInputs>();

    public NetworkLevelController(ILocalInputSource localInputSource) {
      _localInputSource = localInputSource;
    }

    public void SessionStarted(ServerMsgStart msgStart) {
      _side = (Side) msgStart.YourIndex;
      _tickDuration = SnMath.Div(SnMath.One, msgStart.TicksPerSecond);

      var stateCount = msgStart.TicksPerSecond / 2 + 1;
      _states = new LevelState[stateCount];
      _inputs = new NetworkInputs[stateCount * 2 - 1];

      for (var frame = 0; frame < InputDelay; frame++)
        _inputs[frame].Approved = true;

      var left = new PlayerInfo(msgStart.Players[0], msgStart.YourIndex != 0);
      var right = new PlayerInfo(msgStart.Players[1], msgStart.YourIndex != 1);

      var tickDuration = SnMath.Div(SnMath.One, msgStart.TicksPerSecond);
      var randomState = Pcg.CreateState(new Random(msgStart.Seed));
      var simSettings = new SimulationSettings(tickDuration, randomState);

      var settings = new LevelSettings(left, right, simSettings);

      _model = new LevelModel(settings);
      _states[0] = _model.CreateInitialState();

      _view = new LevelView(_states[0], settings);
      _playingTime = Stopwatch.StartNew();
    }

    public void InputReceived(ServerMsgInput msgInput) {
      if (msgInput.PlayerIndex == (byte) _side)
        throw new Exception("My side inputs received");

      var (min, max) = GetInputFramesRangeInclusive();
      if (msgInput.Frame < min || msgInput.Frame > max || msgInput.Frame < InputDelay)
        throw new Exception($"Failed to write frame input {msgInput.Frame} ({min}, {max})");

      var msgKeys = (Keys) msgInput.InputMask;

      for (var frame = msgInput.Frame; frame <= max; frame++) {
        ref var input = ref _inputs[frame % _inputs.Length];

        if (input.Approved) throw new Exception("Try to rewrite approved input");

        ref var enemySideKeys = ref msgInput.PlayerIndex == 0 ? ref input.Left : ref input.Right;
        if (frame == msgInput.Frame) {
          input.Approved = true;
          if (frame < _frame)
            input.MisPredicted = msgKeys != enemySideKeys; // Was simulated with bad value
        }

        enemySideKeys = msgKeys; // Primitive input state prediction
      }
    }

    public (Queue<ClientMsgInputs>, ClientMsgFinished?) Process() {
      ClientMsgFinished? finishMsg = null;
      if (_finished)
        return (_inputSendQueue, null);

      var simulated = false;
      for (var frame = GetFirstMisPredictedFrame(); frame < _frame; frame++) {
        _finished = SimulateFrame(frame);
        HandleFrameResimualated(frame);

        simulated = true;
        if (_finished) break;
      }

      var targetFrame = _finished ? _frame : GetTargetSimulationFrame();
      for (; _frame < targetFrame;) {
        PushLocalInputs(_side == Side.Left ? _localInputSource.GetLeft() : _localInputSource.GetRight());
        _finished = SimulateFrame(_frame++);
        HandleFrameIncremented();

        simulated = true;
        if (_finished) break;
      }

      if (_finished)
        finishMsg = new ClientMsgFinished(_frame, _states[_frame % _states.Length].GetHashCode());

      if (simulated) {
        var prevState = _states[(_frame - 1) % _states.Length];
        var currState = _states[_frame % _states.Length];
        _view.StateContainer.SetPreviousAndCurrentStates(prevState, currState);
      }

      return (_inputSendQueue, finishMsg);
    }

    private uint GetTargetSimulationFrame() {
      var approvedFrames = 0u;
      foreach (var input in _inputs) {
        if (!input.Approved)
          break;
        approvedFrames++;
      }

      var maxReachableFrame = Math.Max((uint) _states.Length, _frame + approvedFrames);
      var timeBasedFrame = (uint) (_playingTime.ElapsedMilliseconds / _tickDuration);

      return Math.Min(timeBasedFrame, maxReachableFrame);
    }

    private (uint, uint) GetInputFramesRangeInclusive() {
      var window = (uint) _inputs.Length / 2;
      return _frame < window ? (0, (uint) _inputs.Length - 1) : (_frame - window, _frame + window);
    }

    private uint GetFirstMisPredictedFrame() {
      var (min, _) = GetInputFramesRangeInclusive();
      for (var frame = min; frame < _frame; frame++) {
        if (_inputs[frame % _inputs.Length].MisPredicted)
          return frame;
      }
      return _frame;
    }

    private void HandleFrameResimualated(uint frame) {
      ref var input = ref _inputs[frame % _inputs.Length];
      if (input.Approved)
        input.MisPredicted = false;
    }

    private void HandleFrameIncremented() {
      var (_, max) = GetInputFramesRangeInclusive();
      var prevMax = _inputs[max - 1 % _inputs.Length];
      _inputs[max % _inputs.Length] = new NetworkInputs {
        Left = prevMax.Left,
        Right = prevMax.Right,
        Approved = false,
        MisPredicted = false
      };
    }

    private bool SimulateFrame(uint frame) {
      var state = _states[frame % _states.Length];
      var inputs = _inputs[frame % _inputs.Length];

      var finished = _model.Tick(ref state, inputs.Left, inputs.Right);
      _states[(frame + 1) % _states.Length] = state;

      return finished;
    }

    private void PushLocalInputs(Keys keys) {
      var frame = _frame + InputDelay;
      _inputSendQueue.Enqueue(new ClientMsgInputs(frame, (ulong) keys));

      var input = _inputs[frame % _inputs.Length];
      ref var mySideKeys = ref _side == Side.Left ? ref input.Left : ref input.Right;
      mySideKeys = keys;
    }

    public void SessionFinished(ServerMsgFinish msgFinish) {
      _view.ShowSessionFinished(msgFinish.Frames, msgFinish.Hashes);
    }

    public void SessionClosedByServerError(ServerErrorId errorId) {
      _view.ShowSessionClosed($"Disconnected by server: {errorId}");
    }

    public void SessionClosedByConnectionError() {
      _view.ShowSessionClosed("Connection error");
    }

    public void SessionClosedByProtocolError() {
      _view.ShowSessionClosed("Protocol error");
    }

    public void SessionClosedByInternalError() {
      _view.ShowSessionClosed("Internal client error");
    }

    public void Dispose() => _view?.Dispose();
  }
}