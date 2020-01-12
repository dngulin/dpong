using System;
using DPong.Level.AI;
using DPong.Level.Data;
using DPong.Level.Model;
using DPong.Level.State;
using DPong.Level.View;

namespace DPong.Level {
  public class LocalLevelController : IDisposable {
    private readonly bool _leftIsBot;
    private readonly bool _rightIsBot;

    private readonly ILocalInputSource _localInputSrc;
    private readonly AiInputSource _aiInputSrc;
    private readonly LevelModel _model;
    private readonly LevelView _view;

    private LevelState _state;

    private bool _finished;

    public LocalLevelController(LevelSettings settings, ILocalInputSource localInputSrc) {
      _leftIsBot = settings.PlayerLeft.Type == PlayerType.Bot;
      _rightIsBot = settings.PlayerRight.Type == PlayerType.Bot;

      _localInputSrc = localInputSrc;
      _aiInputSrc = new AiInputSource();

      _model = new LevelModel(settings);
      _state = _model.CreateInitialState();

      _view = new LevelView(_state, settings);
    }

    public void Tick() {
      if (_finished) return;

      var leftKeys = _leftIsBot ? _aiInputSrc.GetLeft(_state) : _localInputSrc.GetLeft();
      var rightKeys = _rightIsBot ? _aiInputSrc.GetRight(_state) : _localInputSrc.GetRight();

      _finished = _model.Tick(ref _state, leftKeys, rightKeys);
      _view.StateContainer.PushNextState(_state);
    }

    public void Dispose() {
      _view?.Dispose();
    }
  }
}