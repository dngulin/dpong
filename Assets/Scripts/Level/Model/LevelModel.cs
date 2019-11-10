using DPong.Level.Data;
using DPong.Level.State;
using PGM.Collisions;
using PGM.Collisions.Shapes2D;
using PGM.Random;
using PGM.ScaledNum;
using PGM.SceneGraph;

namespace DPong.Level.Model {
  public class LevelModel {
    private const int Goal = 5;
    private const long BlockerSpeed = 7_000;
    private const long BallSpeed = 8_000;
    private const long FreezeTime = 3_000;

    private const long BoardWidth = 30_000;
    private const long BoardHeight = 20_000;
    private const long BallRadius = 0_500;

    private const long SpeedFactorInc = 0_100;
    private const long SpeedFactorMax = 2_700;

    private static readonly SnVector2 DefaultBallSpeed = SnVector2.Mul(SnVector2.Up, BallSpeed);

    private static readonly SnVector2 GateSize = new SnVector2(10_000, 40_000);
    private static readonly SnVector2 BlockerSize = new SnVector2(1_250, 5_000);
    private static readonly SnVector2 MarginSize = new SnVector2(30_000, 10_000);

    private static readonly SnVector2 GatePos = new SnVector2((BoardWidth + GateSize.X) / 2, 0);
    private static readonly SnVector2 BlockerPos = new SnVector2(BoardWidth / 2, 0);
    private static readonly SnVector2 MarginPos = new SnVector2(0, (BoardHeight + MarginSize.Y) / 2);

    private static readonly long MaxBlockerDeviation = (BoardHeight - BlockerSize.Y) / 2;

    private static readonly ColliderState GateLeft = CreateRectCollider(-GatePos, GateSize);
    private static readonly ColliderState GateRight = CreateRectCollider(GatePos, GateSize);

    private static readonly ColliderState MarginDown = CreateRectCollider(-MarginPos, MarginSize);
    private static readonly ColliderState MarginUp = CreateRectCollider(MarginPos, MarginSize);

    private readonly long _frameTime;

    public LevelModel(SimulationSettings settings) {
      _frameTime = settings.FrameTime;
    }

    public static LevelState CreateInitialState() {
      return new LevelState {
        Random = Pcg.CreateState(),
        SpeedFactor = SnMath.One,

        LeftScore = 0,
        RightScore = 0,

        FreezeTime = FreezeTime,
        BallSpeed = DefaultBallSpeed,
        Ball = CreateCircleCollider(SnVector2.Zero, BallRadius),

        LeftBlocker = CreateRectCollider(-BlockerPos, BlockerSize),
        RightBlocker = CreateRectCollider(BlockerPos, BlockerSize)
      };
    }

    public void Tick(ref LevelState state, Keys leftKeys, Keys rightKeys) {
      if (IsFinished(ref state))
        return;

      MoveBlocker(ref state.LeftBlocker, leftKeys, state.SpeedFactor);
      MoveBlocker(ref state.RightBlocker, rightKeys, state.SpeedFactor);

      if (state.FreezeTime > 0) {
        UpdateFreezeTime(ref state);
        return;
      }

      MoveBall(ref state.Ball, state.BallSpeed, state.SpeedFactor);
      CheckCollisions(ref state);
    }

    private void MoveBlocker(ref ColliderState blocker, Keys keys, long speedFactor) {
      if (keys == Keys.None || keys == (Keys.Up | Keys.Down))
        return;

      var directedSpeed = BlockerSpeed * (keys.HasKey(Keys.Up) ? 1 : -1);
      var offsetMultiplier = SnMath.Mul(_frameTime, speedFactor);
      var offset = SnMath.Mul(directedSpeed, offsetMultiplier);

      var oldX = blocker.Pose.Position.X;
      var oldY = blocker.Pose.Position.Y;
      var newY = SnMath.Clamp(oldY + offset, -MaxBlockerDeviation, MaxBlockerDeviation);

      if (oldY == newY)
        return;

      blocker.Pose.Position = new SnVector2(oldX, newY);
      blocker.Transform = Shape2D.GetTransform(blocker.Pose);
    }

    private void MoveBall(ref ColliderState ball, in SnVector2 speedVector, long speedFactor) {
      var offsetMultiplier = SnMath.Mul(_frameTime, speedFactor);
      var offset = SnVector2.Mul(speedVector, offsetMultiplier);

      var oldPosition = ball.Pose.Position;
      var newPosition = ball.Pose.Position + offset;

      if (oldPosition == newPosition)
        return;

      ball.Pose.Position = newPosition;
      ball.Transform = Shape2D.GetTransform(ball.Pose);
    }

    private void UpdateFreezeTime(ref LevelState state) {
      state.FreezeTime -= _frameTime;
      if (state.FreezeTime > 0)
        return;

      state.FreezeTime = 0;

      var angle = SnMath.DegToRad(45_000) + Pcg.NextRanged(ref state.Random, 0, 4) * SnMath.DegToRad(90_000);
      state.BallSpeed = DefaultBallSpeed * Transform.Combine(SnVector2.Zero, angle);
    }

    private void CheckCollisions(ref LevelState state) {
      if (Collision2D.Check(state.Ball.ToShapeState(), GateLeft.ToShapeState(), SnVector2.Left)) {
        state.LeftScore += 1;
        HandleGoal(ref state);
        return;
      }

      if (Collision2D.Check(state.Ball.ToShapeState(), GateRight.ToShapeState(), SnVector2.Right)) {
        state.RightScore += 1;
        HandleGoal(ref state);
        return;
      }

      // Orthodox order!
      CheckBounce(ref state, MarginUp);
      CheckBounce(ref state, MarginDown);
      CheckBounce(ref state, state.RightBlocker);
      CheckBounce(ref state, state.LeftBlocker);
    }

    private void HandleGoal(ref LevelState state) {
      state.FreezeTime = FreezeTime;
      state.SpeedFactor = SnMath.One;
      state.Ball.Pose.Position = SnVector2.Zero;
      state.Ball.Transform = Shape2D.GetTransform(state.Ball.Pose);
    }

    private unsafe void CheckBounce(ref LevelState state, ColliderState collider) {
      var dir = Shape2D.GetCenter(state.Ball.Transform) - Shape2D.GetCenter(collider.Transform);
      var ball = state.Ball.ToShapeState();
      var blocker = collider.ToShapeState();
      if (!Collision2D.Check(ball, blocker, dir))
        return;

      state.SpeedFactor = SnMath.Clamp(state.SpeedFactor + SpeedFactorInc, SnMath.One, SpeedFactorMax);

      var penetrations = stackalloc SnVector2[4];
      penetrations[0] = Collision2D.GetPenetration(ball, blocker, SnVector2.Up);
      penetrations[1] = Collision2D.GetPenetration(ball, blocker, SnVector2.Left);
      penetrations[2] = Collision2D.GetPenetration(ball, blocker, SnVector2.Down);
      penetrations[3] = Collision2D.GetPenetration(ball, blocker, SnVector2.Right);

      var minPenetration = long.MaxValue;
      var minIndex = 0;
      for (var i = 0; i < 4; i++) {
        var penetration = penetrations[i].SquareMagnitude();
        if (penetration >= minPenetration)
          continue;

        minPenetration = penetration;
        minIndex = i;
      }

      state.Ball.Pose.Position -= penetrations[minIndex];
      state.Ball.Transform = Shape2D.GetTransform(state.Ball.Pose);

      var isVertical = minIndex % 2 == 0;
      var speed = state.BallSpeed;
      state.BallSpeed = isVertical ? new SnVector2(speed.X, -speed.Y) : new SnVector2(-speed.X , speed.Y);

      var angle = SnMath.DegToRad(Pcg.NextRanged(ref state.Random, 0, 10_000)) - SnMath.DegToRad(5_000);
      state.BallSpeed *= Transform.Combine(SnVector2.Zero, angle);
    }

    private bool IsFinished(ref LevelState state) {
      return state.LeftScore >= Goal || state.RightScore >= Goal;
    }

    private static ColliderState CreateRectCollider(in SnVector2 pos, in SnVector2 size) {
      var rect = new ColliderState {
        Shape = ShapeType2D.Rect,
        Size = new ShapeSize2D(new RectSize2D(size.X, size.Y)),
        Pose = new ShapePose2D {
          Position = pos,
          Rotation = 0,
          Offset = SnVector2.Zero
        }
      };
      rect.Transform = Shape2D.GetTransform(rect.Pose);
      return rect;
    }

    private static ColliderState CreateCircleCollider(in SnVector2 pos, long r) {
      var rect = new ColliderState {
        Shape = ShapeType2D.Circle,
        Size = new ShapeSize2D(new CircleSize2D(r)),
        Pose = new ShapePose2D {
          Position = pos,
          Rotation = 0,
          Offset = SnVector2.Zero
        }
      };
      rect.Transform = Shape2D.GetTransform(rect.Pose);
      return rect;
    }
  }
}