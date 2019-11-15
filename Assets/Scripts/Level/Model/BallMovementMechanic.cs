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

    public bool TryMove(ref DynamicLevelState dynState) {
      if (dynState.FreezeTime > 0) {
        UpdateFreezeTime(ref dynState);
        return false;
      }

      var multiplier = SnMath.Mul(_stState.TickDuration, dynState.SpeedFactor);
      dynState.Ball.Position += SnVector2.Mul(dynState.BallSpeed, multiplier);
      return true;
    }

    public void Freeze(ref DynamicLevelState dynState) {
      dynState.FreezeTime = _stState.FreezeTime;
      dynState.Ball.Position = SnVector2.Zero;
    }

    public void Shift(ref DynamicLevelState dynState, in SnVector2 shift) {
      dynState.Ball.Position += shift;
    }

    public void Bounce(ref DynamicLevelState dynState, in SnVector2 normal) {
      var dot = SnVector2.Dot(dynState.BallSpeed, normal);
      dynState.BallSpeed -= SnVector2.Mul(normal, dot * 2);

      var angle = SnMath.DegToRad(Pcg.NextRanged(ref dynState.Random, 0, 10_000)) - SnMath.DegToRad(5_000);
      dynState.BallSpeed *= Transform.Combine(SnVector2.Zero, angle);
    }

    private void UpdateFreezeTime(ref DynamicLevelState dynState) {
      dynState.FreezeTime -= _stState.TickDuration;
      if (dynState.FreezeTime > 0)
        return;

      dynState.FreezeTime = 0;
      SetRandomMovementDirection(ref dynState);
    }

    private void SetRandomMovementDirection(ref DynamicLevelState dynState) {
      var angle = SnMath.DegToRad(45_000) + Pcg.NextRanged(ref dynState.Random, 0, 4) * SnMath.DegToRad(90_000);
      dynState.BallSpeed = _defaultBallSpeed * Transform.Combine(SnVector2.Zero, angle);
    }
  }
}