using DPong.Level.AI;
using DPong.Level.Data;
using DPong.Level.Model;
using DPong.Level.State;
using DPong.Level.View;

namespace DPong.Level {
  public class LocalLevelController {
    private readonly bool _leftIsBot;
    private readonly bool _rightIsBot;

    private readonly ILocalInputSource _localInputSrc;
    private readonly AiInputSource _aiInputSrc;
    private readonly LevelModel _model;
    private readonly LevelView _view;

    private DynamicLevelState _state;

    public LocalLevelController(LevelInfo info, ILocalInputSource localInputSrc) {
      _leftIsBot = info.Left.IsBot;
      _rightIsBot = info.Right.IsBot;

      _localInputSrc = localInputSrc;
      _aiInputSrc = new AiInputSource();

      var staticState = new StaticLevelState(info);

      _model = new LevelModel(staticState, info.Settings.RandomState);
      _state = _model.CreateInitialState();

      _view = new LevelView(_state, staticState);
    }

    public void Tick() {
      var leftKeys = _leftIsBot ? _aiInputSrc.GetLeft(_state) : _localInputSrc.GetLeft();
      var rightKeys = _rightIsBot ? _aiInputSrc.GetRight(_state) : _localInputSrc.GetRight();

      _model.Tick(ref _state, leftKeys, rightKeys);

      _view.StateContainer.PushNextState(_state);
    }
  }
}