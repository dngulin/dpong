using System;
using DPong.Common;
using DPong.Level.AI;
using DPong.Level.Data;
using DPong.Level.Model;
using DPong.Level.State;
using DPong.Level.UI;
using DPong.Level.View;
using DPong.UI;

namespace DPong.Level {
  public class LocalLevel : ILevelUIListener, IDisposable {
    private readonly AiInputSource _aiInputSrc;
    private readonly IInputSource _lInputSrc;
    private readonly IInputSource _rInoutSrc;

    private readonly LevelModel _model;
    private readonly LevelView _view;
    private readonly LevelUI _ui;

    private readonly SimulationsTimer _timer;

    private LevelState _previousState;
    private LevelState _state;

    private bool _paused;
    private bool _finished;

    private readonly ILevelExitListener _exitListener;

    public LocalLevel(LevelSettings settings, IInputSource lInputSrc, IInputSource rInputSrc, UISystem uiSystem, ILevelExitListener exitListener) {
      _exitListener = exitListener;
      _aiInputSrc = new AiInputSource();

      _lInputSrc = settings.PlayerLeft.Type == PlayerType.Local ? lInputSrc : null;
      _rInoutSrc = settings.PlayerRight.Type == PlayerType.Local ? rInputSrc : null;

      _model = new LevelModel(settings);
      _state = _model.CreateInitialState();
      _previousState = _state;

      _view = new LevelView(_state, settings);
      _ui = new LevelUI(uiSystem, this);

      _timer = new SimulationsTimer(settings.Simulation.TickDuration.Unscaled());
    }

    public void Tick(float dt) {
      if (_finished || _paused) return;

      var (simulations, blendingFactor) = _timer.Tick(dt);

      for (var i = 0; i < simulations; i++) {
        var leftKeys = _lInputSrc?.GetKeys() ?? _aiInputSrc.GetLeft(_state);
        var rightKeys = _rInoutSrc?.GetKeys() ?? _aiInputSrc.GetRight(_state);

        _previousState = _state;
        _finished = _model.Tick(ref _state, leftKeys, rightKeys);

        if (_finished)
          break;
      }

      _view.UpdateState(_previousState, _state, blendingFactor);

      if (_finished)
        _ui.ShowResult(_state.HitPoints);
    }

    public void Dispose() {
      _view.Dispose();
      _ui.Dispose();
    }

    void ILevelUIListener.PauseCLicked() => _paused = true;
    void ILevelUIListener.ResumeCLicked() => _paused = false;

    void ILevelUIListener.ExitCLicked() => _exitListener.Exit();
  }
}