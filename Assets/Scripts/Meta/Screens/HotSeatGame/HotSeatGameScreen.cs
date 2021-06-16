using System;
using DPong.Level;
using DPong.Level.Data;
using DPong.StateTracker;

namespace DPong.Meta.Screens.HotSeatGame {
  public class HotSeatGameScreen : GameState<DPong> {
    private readonly LocalLevel _level;

    public HotSeatGameScreen(LevelSettings levelSettings, IInputSource[] inputSources, DPong game) {
      var levelViewFactory = new LevelViewFactory(game.Assets, game.Ui);
      _level = new LocalLevel(levelSettings, inputSources, levelViewFactory);
    }

    public override void Start(DPong game) {
    }

    public override Transition Tick(DPong game, float dt) {
      var finished = _level.Tick(dt);
      if (finished)
        return Transition.Pop();

      return Transition.None();
    }

    public override void Pause(DPong game) => throw new NotSupportedException();
    public override void Resume(DPong game) => throw new NotSupportedException();
    public override void Finish(DPong game) => _level.Dispose();
  }
}