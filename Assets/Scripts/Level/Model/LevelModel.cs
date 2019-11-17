using DPong.Level.Data;
using DPong.Level.State;
using PGM.Random;

namespace DPong.Level.Model {
  public class LevelModel {
    private static readonly Side[] Sides = {Side.Left, Side.Right};

    private readonly PcgState _initialRandomState;

    private readonly HitPointsMechanic _hitPoints;
    private readonly PaceMechanic _pace;
    private readonly BlockersMechanic _blockers;
    private readonly BallMechanic _ball;
    private readonly CollisionsMechanic _collisions;

    public LevelModel(LevelSettings settings) {
      _initialRandomState = settings.Simulation.RandomState ?? Pcg.CreateState();

      _hitPoints = new HitPointsMechanic(settings.HitPoints);
      _pace = new PaceMechanic(settings.Pace);
      _blockers = new BlockersMechanic(settings.Blocker, settings.Board.Size, settings.Simulation.TickDuration);
      _ball = new BallMechanic(settings.Ball, settings.Simulation.TickDuration);
      _collisions = new CollisionsMechanic(settings.Board);
    }

    public LevelState CreateInitialState() {
      return new LevelState {
        Random = _initialRandomState,
        Pace = _pace.Default,
        HitPoints = _hitPoints.InitialState,
        Ball = _ball.InitialState,
        Blockers = _blockers.InitialState
      };
    }

    public void Tick(ref LevelState state, Keys leftKeys, Keys rightKeys) {
      if (_hitPoints.IsLevelCompleted(state.HitPoints)) return;

      var (lBlocker, rBlocker) = _blockers.Move(ref state.Blockers, state.Pace, leftKeys, rightKeys);
      var ballMoved = _ball.TryMove(ref state.Ball, ref state.Random, state.Pace);

      if (!ballMoved) return;

      MakeBounceCollisions(ref state, lBlocker, rBlocker);
      CheckGates(ref state);
    }

    private unsafe void MakeBounceCollisions(ref LevelState state, in BounceObj lBlocker, in BounceObj rBlocker) {
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
    }

    private void CheckGates(ref LevelState state) {
      var ballShape = _ball.GetShape(ref state.Ball);

      foreach (var side in Sides) {
        if (!_collisions.CheckGates(ballShape, side)) continue;

        _hitPoints.DecreaseHp(ref state.HitPoints, side);
        _ball.Freeze(ref state.Ball);

        state.Pace = _pace.Default;
        break;
      }
    }
  }
}