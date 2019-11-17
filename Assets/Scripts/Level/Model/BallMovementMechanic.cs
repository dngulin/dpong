using DPong.Level.State;
using PGM.Random;
using PGM.ScaledNum;
using PGM.SceneGraph;

namespace DPong.Level.Model {
  public class BallMovementMechanic {
    private readonly StaticLevelState _stState;
    private readonly SnVector2 _defaultBallSpeed;

    public SnVector2 DefaultBallSpeed => _defaultBallSpeed;

    public BallMovementMechanic(StaticLevelState stState) {
      _stState = stState;
      _defaultBallSpeed = SnVector2.Mul(SnVector2.Up, stState.BallSpeed);
    }

    public bool TryMove(ref LevelState state) {
      if (state.FreezeTime > 0) {
        UpdateFreezeTime(ref state);
        return false;
      }

      var multiplier = SnMath.Mul(_stState.TickDuration, state.SpeedFactor);
      state.Ball.Position += SnVector2.Mul(state.BallSpeed, multiplier);
      return true;
    }

    public void Freeze(ref LevelState state) {
      state.FreezeTime = _stState.FreezeTime;
      state.Ball.Position = SnVector2.Zero;
    }

    public void Shift(ref LevelState state, in SnVector2 shift) {
      state.Ball.Position += shift;
    }

    public void Bounce(ref LevelState state, in SnVector2 normal, in SnVector2 movement) {
      var dot = SnVector2.Dot(state.BallSpeed, normal);
      state.BallSpeed -= SnVector2.Mul(normal, dot * 2);

      var deviation = _stState.BounceDeviationDegrees;
      var angle = SnMath.DegToRad(Pcg.NextRanged(ref state.Random, -deviation, deviation));
      state.BallSpeed *= Transform.Combine(SnVector2.Zero, angle);
    }

    private void UpdateFreezeTime(ref LevelState state) {
      state.FreezeTime -= _stState.TickDuration;
      if (state.FreezeTime > 0)
        return;

      state.FreezeTime = 0;
      SetRandomMovementDirection(ref state);
    }

    private void SetRandomMovementDirection(ref LevelState state) {
      var angle = SnMath.DegToRad(45_000) + Pcg.NextRanged(ref state.Random, 0, 4) * SnMath.DegToRad(90_000);
      state.BallSpeed = _defaultBallSpeed * Transform.Combine(SnVector2.Zero, angle);
    }
  }
}