using DPong.Level.Data;
using DPong.Level.State;
using PGM.Random;
using PGM.ScaledNum;

namespace DPong.Level.Model {
  public class LevelModel {
    private static readonly Side[] Sides = {Side.Left, Side.Right};

    private readonly StaticLevelState _stState;
    private readonly PcgState _initialPcgState;

    private readonly ProgressMechanic _progress;
    private readonly PaceMechanic _pace;
    private readonly BlockersMechanic _blockers;
    private readonly BallMechanic _ball;
    private readonly CollisionsMechanic _collisions;

    public LevelModel(StaticLevelState stState, PcgState? pcgState) {
      _stState = stState;
      _initialPcgState = pcgState ?? Pcg.CreateState();

      _progress = new ProgressMechanic(stState);
      _pace = new PaceMechanic(stState);
      _blockers = new BlockersMechanic(stState);
      _ball = new BallMechanic(stState);
      _collisions = new CollisionsMechanic(stState);
    }

    public LevelState CreateInitialState() {
      var blockerPos = new SnVector2(_stState.BoardSize.Width / 2, 0);

      return new LevelState {
        Random = _initialPcgState,
        SpeedFactor = _pace.Default,

        LeftHp = _progress.DefaultHp,
        RightHp = _progress.DefaultHp,

        FreezeTime = _stState.FreezeTime,
        BallSpeed = _ball.DefaultBallSpeed,

        Ball = new ColliderState(SnVector2.Zero),
        LeftBlocker = new ColliderState(-blockerPos),
        RightBlocker = new ColliderState(blockerPos)
      };
    }

    public unsafe void Tick(ref LevelState state, Keys leftKeys, Keys rightKeys) {
      if (_progress.IsLevelCompleted(state))
        return;

      var lBlocker = _blockers.Move(ref state.LeftBlocker, leftKeys, state.SpeedFactor);
      var rBlocker = _blockers.Move(ref state.RightBlocker, rightKeys, state.SpeedFactor);

      if (!_ball.TryMove(ref state))
        return;

      const int objCount = 4;
      var objects = stackalloc BounceObj[objCount] {lBlocker, rBlocker, _collisions.BorderUp, _collisions.BorderDown};

      for (var objIdx = 0; objIdx < objCount; objIdx++) {
        if (!_collisions.Check(_ball.GetShape(ref state), objects[objIdx].State, out var penetration))
          continue;

        _ball.Shift(ref state, -penetration);
        _ball.Bounce(ref state, -penetration.Normalized(), objects[objIdx].Movement.Normalized());
        state.SpeedFactor = _pace.SpeedUp(state.SpeedFactor);
      }

      foreach (var side in Sides) {
        if (!_collisions.CheckGates(_ball.GetShape(ref state), side))
          continue;

        _progress.DecreaseHp(ref state, side);
        _ball.Freeze(ref state);
        state.SpeedFactor = _pace.Default;
        break;
      }
    }
  }
}