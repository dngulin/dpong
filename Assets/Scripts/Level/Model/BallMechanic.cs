using System;
using DPong.Common;
using DPong.Level.State;
using PGM.Collisions.Shapes2D;
using PGM.Random;
using PGM.ScaledNum;
using PGM.SceneGraph;

namespace DPong.Level.Model {
  public class BallMechanic {
    private readonly StaticLevelState _stState;
    private readonly SnVector2 _defaultBallSpeed;
    private readonly ShapeSize2D _size;

    public readonly BallState InitialState;

    public BallMechanic(StaticLevelState stState) {
      _stState = stState;
      _defaultBallSpeed = SnVector2.Mul(SnVector2.Up, stState.BallSpeed);
      _size = new ShapeSize2D(_stState.BallSize);

      InitialState = new BallState {
        FreezeCooldown = stState.FreezeTime,
        Speed = _defaultBallSpeed,
        Position = SnVector2.Zero
      };
    }

    public ShapeState2D GetShape(ref BallState ball) {
      return new ShapeState2D(ShapeType2D.Circle, _size, Transform.Translate(ball.Position));
    }

    public bool TryMove(ref BallState ball, ref PcgState random, long pace) {
      if (ball.FreezeCooldown > 0) {
        UpdateCooldown(ref ball, ref random);
        return false;
      }

      var multiplier = SnMath.Mul(_stState.TickDuration, pace);
      ball.Position += SnVector2.Mul(ball.Speed, multiplier);
      return true;
    }

    public void Freeze(ref BallState ball) {
      ball.FreezeCooldown = _stState.FreezeTime;
      ball.Position = SnVector2.Zero;
    }

    public void Shift(ref BallState ball, in SnVector2 shift) {
      ball.Position += shift;
    }

    public void Bounce(ref BallState ball, ref PcgState random, in SnVector2 bounceNorm, in SnVector2 movementNorm) {
      // Reflect speed vector
      var dot = SnVector2.Dot(ball.Speed, bounceNorm);
      ball.Speed -= SnVector2.Mul(bounceNorm, dot * 2);

      // Calculate rotation by blocker movement
      var cross = SnVector3.Cross(movementNorm.To3D(), bounceNorm.To3D());
      var movementAngle = Math.Sign(cross.Z) * SnMath.DegToRad(_stState.BounceMovementAngle);

      // Calculate some random rotation
      var deviation = _stState.BounceDeviationDegrees;
      var deviationAngle = SnMath.DegToRad(Pcg.NextRanged(ref random, -deviation, deviation));

      // Apply speed vector rotation
      ball.Speed *= Transform.Combine(SnVector2.Zero, movementAngle + deviationAngle);
    }

    private void UpdateCooldown(ref BallState ball, ref PcgState random) {
      ball.FreezeCooldown -= _stState.TickDuration;
      if (ball.FreezeCooldown > 0)
        return;

      ball.FreezeCooldown = 0;
      SetRandomMovementDirection(ref ball, ref random);
    }

    private void SetRandomMovementDirection(ref BallState ball, ref PcgState random) {
      var angle = SnMath.DegToRad(45_000) + Pcg.NextRanged(ref random, 0, 4) * SnMath.DegToRad(90_000);
      ball.Speed = _defaultBallSpeed * Transform.Combine(SnVector2.Zero, angle);
    }
  }
}