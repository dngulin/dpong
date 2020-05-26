using System;
using DPong.Level.AI;
using DPong.Level.Data;
using DPong.Level.Model;
using DPong.Level.State;
using DPong.Level.UI;
using DPong.Level.View;
using DPong.UI;

namespace DPong.Level {
  public class LocalLevelController : ILevelUIListener, IDisposable {
    private readonly AiInputSource _aiInputSrc;
    private readonly IInputSource _lInputSrc;
    private readonly IInputSource _rInoutSrc;

    private readonly LevelModel _model;
    private readonly LevelView _view;
    private readonly LevelUI _ui;

    private LevelState _state;

    private bool _paused;
    private bool _finished;

    public LocalLevelController(LevelSettings settings, IInputSource lInputSrc, IInputSource rInputSrc, UISystem uiSystem) {
      _aiInputSrc = new AiInputSource();
      _lInputSrc = settings.PlayerLeft.Type == PlayerType.Local ? lInputSrc : null;
      _rInoutSrc = settings.PlayerRight.Type == PlayerType.Local ? rInputSrc : null;

      _model = new LevelModel(settings);
      _state = _model.CreateInitialState();

      _view = new LevelView(_state, settings);
      _ui = new LevelUI(uiSystem, this);
    }

    public void Tick() {
      if (_finished || _paused) return;

      var leftKeys = _lInputSrc?.GetKeys() ?? _aiInputSrc.GetLeft(_state);
      var rightKeys = _rInoutSrc?.GetKeys() ?? _aiInputSrc.GetRight(_state);

      _finished = _model.Tick(ref _state, leftKeys, rightKeys);
      _view.StateContainer.PushNextState(_state);

      if (_finished)
        _ui.ShowResult(_state.HitPoints);
    }

    public void Dispose() {
      _view.Dispose();
    }

    void ILevelUIListener.PauseCLicked() => _paused = true;
    void ILevelUIListener.ResumeCLicked() => _paused = false;

    void ILevelUIListener.ExitCLicked()
    {
      throw new NotImplementedException();
    }
  }
}