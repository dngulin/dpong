using System;
using DPong.Level;
using DPong.Level.Data;
using DPong.StateTracker;

namespace DPong.Meta.Screens.HotSeatGame {
  public class HotSeatGameScreen : GameState<DPong>, ILevelExitListener {
    private readonly LocalLevel _level;

    private bool _pendingExit;

    public HotSeatGameScreen(LevelSettings levelSettings, IInputSource[] inputSources, DPong game) {
      var levelViewFactory = new LevelViewFactory(game.Assets, game.Ui);
      _level = new LocalLevel(levelSettings, inputSources, levelViewFactory, this);
    }

    void ILevelExitListener.Exit() => _pendingExit = true;

    public override void Start(DPong game) {
    }

    public override Transition Tick(DPong game, float dt) {
      if (_pendingExit)
        return Transition.Pop();

      _level.Tick(dt);
      return Transition.None();
    }

    public override void Pause(DPong game) => throw new NotSupportedException();

    public override void Resume(DPong game) => throw new NotSupportedException();

    public override void Finish(DPong game) => _level.Dispose();
  }
}