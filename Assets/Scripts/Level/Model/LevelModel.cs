using DPong.Level.Data;
using DPong.Level.State;
using PGM.Random;
using PGM.ScaledNum;

namespace DPong.Level.Model {
  public class LevelModel {
    private static readonly Side[] Sides = {Side.Left, Side.Right};

    private readonly StaticLevelState _stState;
    private readonly PcgState _initialRandomState;

    private readonly HitPointsMechanic _hitPoints;
    private readonly PaceMechanic _pace;
    private readonly BlockersMechanic _blockers;
    private readonly BallMechanic _ball;
    private readonly CollisionsMechanic _collisions;

    public LevelModel(StaticLevelState stState, PcgState? pcgState) {
      _stState = stState;
      _initialRandomState = pcgState ?? Pcg.CreateState();

      _hitPoints = new HitPointsMechanic(stState.HitPoints);
      _pace = new PaceMechanic(stState);
      _blockers = new BlockersMechanic(stState);
      _ball = new BallMechanic(stState);
      _collisions = new CollisionsMechanic(stState);
    }

    public LevelState CreateInitialState() {
      var blockerPos = new SnVector2(_stState.BoardSize.Width / 2, 0);

      return new LevelState {
        Random = _initialRandomState,
        Pace = _pace.Default,

        HitPoints = _hitPoints.InitialState,

        Ball = _ball.InitialState,

        LeftBlocker = new ColliderState(-blockerPos),
        RightBlocker = new ColliderState(blockerPos)
      };
    }

    public unsafe void Tick(ref LevelState state, Keys leftKeys, Keys rightKeys) {
      if (_hitPoints.IsLevelCompleted(state.HitPoints))
        return;

      var lBlocker = _blockers.Move(ref state.LeftBlocker, leftKeys, state.Pace);
      var rBlocker = _blockers.Move(ref state.RightBlocker, rightKeys, state.Pace);

      if (!_ball.TryMove(ref state.Ball, ref state.Random, state.Pace))
        return;

      const int objCount = 4;
      var objects = stackalloc BounceObj[objCount] {lBlocker, rBlocker, _collisions.BorderUp, _collisions.BorderDown};

      for (var objIdx = 0; objIdx < objCount; objIdx++) {
        var obj = objects[objIdx];

        if (!_collisions.Check(_ball.GetShape(ref state.Ball), obj.State, out var penetration))
          continue;

        _ball.Shift(ref state.Ball, -penetration);
        _ball.Bounce(ref state.Ball, ref state.Random, -penetration.Normalized(), obj.Movement.Normalized());

        state.Pace = _pace.SpeedUp(state.Pace);
      }

      foreach (var side in Sides) {
        if (!_collisions.CheckGates(_ball.GetShape(ref state.Ball), side))
          continue;

        _hitPoints.DecreaseHp(ref state.HitPoints, side);
        _ball.Freeze(ref state.Ball);

        state.Pace = _pace.Default;
        break;
      }
    }
  }
}