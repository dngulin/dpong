using DPong.Level.State;
using PGM.Collisions;
using PGM.Collisions.Shapes2D;
using PGM.Random;
using PGM.ScaledNum;
using PGM.SceneGraph;

namespace DPong.Level.Model {
  public class LevelModel {
    private readonly StaticLevelState _stState;
    private readonly PcgState _initialPcgState;

    private readonly SnVector2 _defaultBallSpeed;
    private readonly long _maxBlockerDeviation;

    public LevelModel(StaticLevelState stState, PcgState? pcgState) {
      _stState = stState;
      _initialPcgState = pcgState ?? Pcg.CreateState();

      _defaultBallSpeed = SnVector2.Mul(SnVector2.Up, _stState.BallSpeed);
      _maxBlockerDeviation = (_stState.BoardSize.Height - _stState.BlockerSize.Height) / 2;
    }

    public DynamicLevelState CreateInitialState() {
      var blockerPos = new SnVector2(_stState.BoardSize.Width / 2, 0);

      return new DynamicLevelState {
        Random = _initialPcgState,
        SpeedFactor = SnMath.One,

        LeftHp = _stState.HitPoints,
        RightHp = _stState.HitPoints,

        FreezeTime = _stState.FreezeTime,
        BallSpeed = _defaultBallSpeed,

        Ball = new ColliderState(SnVector2.Zero),
        LeftBlocker = new ColliderState(-blockerPos),
        RightBlocker = new ColliderState(blockerPos)
      };
    }

    public void Tick(ref DynamicLevelState dynState, Keys leftKeys, Keys rightKeys) {
      if (IsFinished(ref dynState))
        return;

      MoveBlocker(ref dynState.LeftBlocker, leftKeys, dynState.SpeedFactor);
      MoveBlocker(ref dynState.RightBlocker, rightKeys, dynState.SpeedFactor);

      if (dynState.FreezeTime > 0) {
        UpdateFreezeTime(ref dynState);
        return;
      }

      MoveBall(ref dynState.Ball, dynState.BallSpeed, dynState.SpeedFactor);
      CheckCollisions(ref dynState);
    }

    private void MoveBlocker(ref ColliderState blocker, Keys keys, long speedFactor) {
      if (keys == Keys.None || keys == (Keys.Up | Keys.Down))
        return;

      var directedSpeed = _stState.BlockerSpeed * (keys.HasKey(Keys.Up) ? 1 : -1);
      var offsetMultiplier = SnMath.Mul(_stState.TickDuration, speedFactor);
      var offset = SnMath.Mul(directedSpeed, offsetMultiplier);

      var oldX = blocker.Position.X;
      var oldY = blocker.Position.Y;
      var newY = SnMath.Clamp(oldY + offset, -_maxBlockerDeviation, _maxBlockerDeviation);

      if (oldY == newY)
        return;

      blocker.Position = new SnVector2(oldX, newY);
    }

    private void MoveBall(ref ColliderState ball, in SnVector2 speedVector, long speedFactor) {
      var offsetMultiplier = SnMath.Mul(_stState.TickDuration, speedFactor);
      var offset = SnVector2.Mul(speedVector, offsetMultiplier);

      var oldPosition = ball.Position;
      var newPosition = ball.Position + offset;

      if (oldPosition == newPosition)
        return;

      ball.Position = newPosition;
    }

    private void UpdateFreezeTime(ref DynamicLevelState dynState) {
      dynState.FreezeTime -= _stState.TickDuration;
      if (dynState.FreezeTime > 0)
        return;

      dynState.FreezeTime = 0;

      var angle = SnMath.DegToRad(45_000) + Pcg.NextRanged(ref dynState.Random, 0, 4) * SnMath.DegToRad(90_000);
      dynState.BallSpeed = _defaultBallSpeed * Transform.Combine(SnVector2.Zero, angle);
    }

    private void CheckCollisions(ref DynamicLevelState dynState) {
      // Orthodox order!
      CheckBounce(ref dynState, _stState.MarginUpper);
      CheckBounce(ref dynState, _stState.MarginDown);
      CheckBounce(ref dynState, dynState.RightBlocker.ToRect(_stState.BlockerSize));
      CheckBounce(ref dynState, dynState.LeftBlocker.ToRect(_stState.BlockerSize));

      if (Collision2D.Check(dynState.Ball.ToCircle(_stState.BallSize), _stState.GateLeft, SnVector2.Left)) {
        dynState.LeftHp--;
        HandleGoal(ref dynState);
      }
      else if (Collision2D.Check(dynState.Ball.ToCircle(_stState.BallSize), _stState.GateRight, SnVector2.Right)) {
        dynState.RightHp--;
        HandleGoal(ref dynState);
      }
    }

    private void HandleGoal(ref DynamicLevelState state) {
      state.FreezeTime = _stState.FreezeTime;
      state.SpeedFactor = SnMath.One;
      state.Ball.Position = SnVector2.Zero;
    }

    private unsafe void CheckBounce(ref DynamicLevelState dynState, in ShapeState2D blocker) {
      var ball = dynState.Ball.ToCircle(_stState.BallSize);
      if (!Collision2D.Check(ball, blocker, dynState.BallSpeed))
        return;

      var speedFactor = dynState.SpeedFactor + _stState.SpeedFactorInc;
      dynState.SpeedFactor = SnMath.Clamp(speedFactor, SnMath.One, _stState.SpeedFactorMax);

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

      dynState.Ball.Position -= penetrations[minIndex];

      var isVertical = minIndex % 2 == 0;
      var speed = dynState.BallSpeed;
      dynState.BallSpeed = isVertical ? new SnVector2(speed.X, -speed.Y) : new SnVector2(-speed.X, speed.Y);

      var angle = SnMath.DegToRad(Pcg.NextRanged(ref dynState.Random, 0, 10_000)) - SnMath.DegToRad(5_000);
      dynState.BallSpeed *= Transform.Combine(SnVector2.Zero, angle);
    }

    private static bool IsFinished(ref DynamicLevelState dynState) {
      return dynState.LeftHp <= 0 || dynState.RightHp <= 0;
    }
  }
}