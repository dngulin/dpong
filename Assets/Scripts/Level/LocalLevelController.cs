using DPong.Common;
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

    private LevelState _state;

    public LocalLevelController(LevelInfo info, ILocalInputSource localInputSrc) {
      _leftIsBot = info.Left.IsBot;
      _rightIsBot = info.Right.IsBot;

      _localInputSrc = localInputSrc;
      _aiInputSrc = new AiInputSource();

      _model = new LevelModel(info.Settings);
      _state = LevelModel.CreateInitialState();

      var frameTime = info.Settings.FrameTime.Unscaled();
      _view = new LevelView(_state, frameTime, info.Left, info.Right);
    }

    public void Tick() {
      var leftKeys = _leftIsBot ? _aiInputSrc.GetLeft(_state) : _localInputSrc.GetLeft();
      var rightKeys = _rightIsBot ? _aiInputSrc.GetRight(_state) : _localInputSrc.GetRight();

      _model.Tick(ref _state, leftKeys, rightKeys);

      _view.StateHolder.PushNextState(_state);
    }
  }
}