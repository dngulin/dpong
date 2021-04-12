using DPong.Level.Data;
using DPong.Level.State;
using FxNet.Random;

namespace DPong.Level.Model {
  public class LevelModel {
    private readonly Side[] _sides = {Side.Left, Side.Right};

    private readonly FxRandomState _initialRandomState;

    private readonly ScoreMechanic _score;
    private readonly PaceMechanic _pace;
    private readonly BlockersMechanic _blockers;
    private readonly BallMechanic _ball;
    private readonly CollisionsMechanic _collisions;

    public LevelModel(LevelSettings settings) {
      _initialRandomState = FxRandom.CreteState(settings.Simulation.Seed);

      _score = new ScoreMechanic(settings.HitPoints);
      _pace = new PaceMechanic(settings.Pace);
      _blockers = new BlockersMechanic(settings.Blocker, settings.Board.Size, settings.Simulation.TickDuration);
      _ball = new BallMechanic(settings.Ball, settings.Simulation.TickDuration);
      _collisions = new CollisionsMechanic(settings.Board);
    }

    public LevelState CreateInitialState() {
      return new LevelState {
        Random = _initialRandomState,
        Pace = _pace.Default,
        Scores = _score.InitialState,
        Ball = _ball.InitialState,
        Blockers = _blockers.InitialState
      };
    }

    public bool Tick(ref LevelState state, Keys leftKeys, Keys rightKeys) {
      if (_score.IsLevelCompleted(state.Scores))
        return true;

      var (lBlocker, rBlocker) = _blockers.Move(ref state, leftKeys, rightKeys);
      var ballMoved = _ball.TryMove(ref state.Ball, ref state.Random, state.Pace);

      if (ballMoved) {
        MakeBounceCollisions(ref state, lBlocker, rBlocker);
        CheckGates(ref state);
      }

      return false;
    }

    private unsafe void MakeBounceCollisions(ref LevelState state, in BounceObj lBlocker, in BounceObj rBlocker) {
      const int objCount = 4;
      var objects = stackalloc BounceObj[objCount] {lBlocker, rBlocker, _collisions.BorderUp, _collisions.BorderDown};

      for (var objIdx = 0; objIdx < objCount; objIdx++) {
        var obj = objects[objIdx];

        if (!_collisions.Check(_ball.GetShape(state.Ball), obj.Shape, out var penetration))
          continue;

        _ball.Shift(ref state.Ball, -penetration);
        _ball.Bounce(ref state.Ball, ref state.Random, obj.BounceInNormal);

        state.Pace = _pace.SpeedUp(state.Pace);
      }
    }

    private void CheckGates(ref LevelState state) {
      var ballShape = _ball.GetShape(state.Ball);

      foreach (var gatesSide in _sides) {
        if (!_collisions.CheckGates(ballShape, gatesSide))
          continue;

        _score.HandleGoal(ref state.Scores, gatesSide);
        _ball.Freeze(ref state.Ball);

        state.Pace = _pace.Default;
        break;
      }
    }
  }
}