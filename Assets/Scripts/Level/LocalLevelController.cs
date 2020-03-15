using System;
using DPong.Level.AI;
using DPong.Level.Data;
using DPong.Level.Model;
using DPong.Level.State;
using DPong.Level.View;

namespace DPong.Level {
  public class LocalLevelController : IDisposable {
    private readonly AiInputSource _aiInputSrc;
    private readonly IInputSource _lInputSrc;
    private readonly IInputSource _rInoutSrc;

    private readonly LevelModel _model;
    private readonly LevelView _view;

    private LevelState _state;

    private bool _finished;

    public LocalLevelController(LevelSettings settings, IInputSource lInputSrc, IInputSource rInputSrc) {
      _aiInputSrc = new AiInputSource();
      _lInputSrc = settings.PlayerLeft.Type == PlayerType.Local ? lInputSrc : null;
      _rInoutSrc = settings.PlayerRight.Type == PlayerType.Local ? rInputSrc : null;

      _model = new LevelModel(settings);
      _state = _model.CreateInitialState();

      _view = new LevelView(_state, settings);
    }

    public void Tick() {
      if (_finished) return;

      var leftKeys = _lInputSrc?.GetKeys() ?? _aiInputSrc.GetLeft(_state);
      var rightKeys = _rInoutSrc?.GetKeys() ?? _aiInputSrc.GetRight(_state);

      _finished = _model.Tick(ref _state, leftKeys, rightKeys);
      _view.StateContainer.PushNextState(_state);
    }

    public void Dispose() {
      _view.Dispose();
    }
  }
}