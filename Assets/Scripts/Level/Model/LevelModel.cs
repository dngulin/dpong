using DPong.Level.State;
using PGM.Collisions;
using PGM.Collisions.Shapes2D;
using PGM.Random;
using PGM.ScaledNum;
using PGM.SceneGraph;

namespace DPong.Level.Model {
  public class LevelModel {
    private readonly StaticLevelState _staticState;
    private readonly PcgState _initialPcgState;

    private readonly SnVector2 _defaultBallSpeed;
    private readonly long _maxBlockerDeviation;

    public LevelModel(StaticLevelState staticState, PcgState? initialPcgState) {
      _staticState = staticState;
      _initialPcgState = initialPcgState ?? Pcg.CreateState();

      _defaultBallSpeed = SnVector2.Mul(SnVector2.Up, _staticState.BallSpeed);
      _maxBlockerDeviation = (_staticState.BoardSize.Height - _staticState.BlockerSize.Height) / 2;
    }

    public DynamicLevelState CreateInitialState() {
      var blockerPos = new SnVector2(_staticState.BoardSize.Width / 2, 0);

      return new DynamicLevelState {
        Random = _initialPcgState,
        SpeedFactor = SnMath.One,

        LeftScore = 0,
        RightScore = 0,

        FreezeTime = _staticState.FreezeTime,
        BallSpeed = _defaultBallSpeed,

        Ball = new ColliderState(SnVector2.Zero),
        LeftBlocker = new ColliderState(-blockerPos),
        RightBlocker = new ColliderState(blockerPos)
      };
    }

    public void Tick(ref DynamicLevelState state, Keys leftKeys, Keys rightKeys) {
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

      var directedSpeed = _staticState.BlockerSpeed * (keys.HasKey(Keys.Up) ? 1 : -1);
      var offsetMultiplier = SnMath.Mul(_staticState.TickDuration, speedFactor);
      var offset = SnMath.Mul(directedSpeed, offsetMultiplier);

      var oldX = blocker.Position.X;
      var oldY = blocker.Position.Y;
      var newY = SnMath.Clamp(oldY + offset, -_maxBlockerDeviation, _maxBlockerDeviation);

      if (oldY == newY)
        return;

      blocker.Position = new SnVector2(oldX, newY);
    }

    private void MoveBall(ref ColliderState ball, in SnVector2 speedVector, long speedFactor) {
      var offsetMultiplier = SnMath.Mul(_staticState.TickDuration, speedFactor);
      var offset = SnVector2.Mul(speedVector, offsetMultiplier);

      var oldPosition = ball.Position;
      var newPosition = ball.Position + offset;

      if (oldPosition == newPosition)
        return;

      ball.Position = newPosition;
    }

    private void UpdateFreezeTime(ref DynamicLevelState state) {
      state.FreezeTime -= _staticState.TickDuration;
      if (state.FreezeTime > 0)
        return;

      state.FreezeTime = 0;

      var angle = SnMath.DegToRad(45_000) + Pcg.NextRanged(ref state.Random, 0, 4) * SnMath.DegToRad(90_000);
      state.BallSpeed = _defaultBallSpeed * Transform.Combine(SnVector2.Zero, angle);
    }

    private void CheckCollisions(ref DynamicLevelState state) {
      // Orthodox order!
      CheckBounce(ref state, _staticState.MarginUpper);
      CheckBounce(ref state, _staticState.MarginDown);
      CheckBounce(ref state, state.RightBlocker.ToRect(_staticState.BlockerSize));
      CheckBounce(ref state, state.LeftBlocker.ToRect(_staticState.BlockerSize));

      if (Collision2D.Check(state.Ball.ToCircle(_staticState.BallSize), _staticState.GateLeft, SnVector2.Left)) {
        state.LeftScore += 1;
        HandleGoal(ref state);
      }
      else if (Collision2D.Check(state.Ball.ToCircle(_staticState.BallSize), _staticState.GateRight, SnVector2.Right)) {
        state.RightScore += 1;
        HandleGoal(ref state);
      }
    }

    private void HandleGoal(ref DynamicLevelState state) {
      state.FreezeTime = _staticState.FreezeTime;
      state.SpeedFactor = SnMath.One;
      state.Ball.Position = SnVector2.Zero;
    }

    private unsafe void CheckBounce(ref DynamicLevelState state, in ShapeState2D blocker) {
      var ball = state.Ball.ToCircle(_staticState.BallSize);
      if (!Collision2D.Check(ball, blocker, state.BallSpeed))
        return;

      var speedFactor = state.SpeedFactor + _staticState.SpeedFactorInc;
      state.SpeedFactor = SnMath.Clamp(speedFactor, SnMath.One, _staticState.SpeedFactorMax);

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

      state.Ball.Position -= penetrations[minIndex];

      var isVertical = minIndex % 2 == 0;
      var speed = state.BallSpeed;
      state.BallSpeed = isVertical ? new SnVector2(speed.X, -speed.Y) : new SnVector2(-speed.X, speed.Y);

      var angle = SnMath.DegToRad(Pcg.NextRanged(ref state.Random, 0, 10_000)) - SnMath.DegToRad(5_000);
      state.BallSpeed *= Transform.Combine(SnVector2.Zero, angle);
    }

    private bool IsFinished(ref DynamicLevelState state) {
      return state.LeftScore >= _staticState.Goal || state.RightScore >= _staticState.Goal;
    }
  }
}