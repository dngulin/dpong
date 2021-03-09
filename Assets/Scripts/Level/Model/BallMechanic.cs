using DPong.Level.Data;
using DPong.Level.State;
using FxNet.Collision2D;
using FxNet.Math;
using FxNet.Random;
using FxNet.SceneGraph;

namespace DPong.Level.Model {
  public class BallMechanic {
    private readonly FxNum _tickDuration;
    private readonly FxNum _bounceDeviationAngle;
    private readonly FxNum _radius;

    public readonly BallState InitialState;

    public BallMechanic(BallSettings settings, in FxNum tickDuration) {
      _tickDuration = tickDuration;

      _radius = settings.Radius;

      _bounceDeviationAngle = settings.BounceRandomAngle * FxMath.Deg2Rad;

      InitialState = new BallState {
        FreezeCooldown = settings.FreezeTime,
        Speed = FxVec2.Up * settings.Speed,
        Position = FxVec2.Zero
      };
    }

    public FxCircle GetShape(in BallState ball) => new FxCircle(ball.Position, _radius);

    public bool TryMove(ref BallState ball, ref FxRandomState random, in FxNum pace) {
      if (ball.FreezeCooldown > 0) {
        UpdateCooldown(ref ball, ref random);
        return false;
      }

      ball.Position += ball.Speed * (_tickDuration * pace);
      return true;
    }

    public void Freeze(ref BallState ball) {
      ball.FreezeCooldown = InitialState.FreezeCooldown;
      ball.Position = FxVec2.Zero;
    }

    public void Shift(ref BallState ball, in FxVec2 shift) {
      ball.Position += shift;
    }

    public void Bounce(ref BallState ball, ref FxRandomState random, in FxVec2 bounceNorm) {
      // Apply speed vector rotation
      ball.Speed = FxVec2.Reflect(ball.Speed, -bounceNorm);

      var angle = random.Next(-_bounceDeviationAngle, _bounceDeviationAngle);
      ball.Speed = FxRigidTransform2D.CombineMatrix(FxVec2.Zero, angle).MultiplyPoint(ball.Speed);
    }

    private void UpdateCooldown(ref BallState ball, ref FxRandomState random) {
      ball.FreezeCooldown -= _tickDuration;
      if (ball.FreezeCooldown > 0)
        return;

      ball.FreezeCooldown = 0;
      SetRandomMovementDirection(ref ball, ref random);
    }

    private void SetRandomMovementDirection(ref BallState ball, ref FxRandomState random) {
      var angle = FxNum.FromRaw(FxMath.PiRaw / 2) * (int) random.Next(0, 4)  + FxNum.FromRaw(FxMath.PiRaw / 4);
      ball.Speed = FxRigidTransform2D.CombineMatrix(FxVec2.Zero, angle).MultiplyPoint(InitialState.Speed);
    }
  }
}