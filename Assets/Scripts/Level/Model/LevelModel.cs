using DPong.Level.Data;
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

    private readonly ProgressionMechanic _progression;
    private readonly BlockerControlMechanic _blockerControl;
    private readonly BallMovementMechanic _ballMovement;

    public LevelModel(StaticLevelState stState, PcgState? pcgState) {
      _stState = stState;
      _initialPcgState = pcgState ?? Pcg.CreateState();

      _progression = new ProgressionMechanic(stState);
      _blockerControl = new BlockerControlMechanic(stState);
      _ballMovement = new BallMovementMechanic(stState);
    }

    public DynamicLevelState CreateInitialState() {
      var blockerPos = new SnVector2(_stState.BoardSize.Width / 2, 0);

      return new DynamicLevelState {
        Random = _initialPcgState,
        SpeedFactor = SnMath.One,

        LeftHp = _progression.DefaultHp,
        RightHp = _progression.DefaultHp,

        FreezeTime = _stState.FreezeTime,
        BallSpeed = _ballMovement.DefaultBallSpeed,

        Ball = new ColliderState(SnVector2.Zero),
        LeftBlocker = new ColliderState(-blockerPos),
        RightBlocker = new ColliderState(blockerPos)
      };
    }

    public void Tick(ref DynamicLevelState dynState, Keys leftKeys, Keys rightKeys) {
      if (_progression.IsLevelCompleted(dynState))
        return;

      _blockerControl.Move(ref dynState.LeftBlocker, leftKeys, dynState.SpeedFactor);
      _blockerControl.Move(ref dynState.RightBlocker, rightKeys, dynState.SpeedFactor);

      if (!_ballMovement.TryMove(ref dynState))
        return;

      CheckCollisions(ref dynState);
    }

    private void CheckCollisions(ref DynamicLevelState dynState) {
      // Orthodox order!
      CheckBounce(ref dynState, _stState.MarginUpper);
      CheckBounce(ref dynState, _stState.MarginDown);
      CheckBounce(ref dynState, dynState.RightBlocker.ToRect(_stState.BlockerSize));
      CheckBounce(ref dynState, dynState.LeftBlocker.ToRect(_stState.BlockerSize));

      if (Collision2D.Check(dynState.Ball.ToCircle(_stState.BallSize), _stState.GateLeft, SnVector2.Left)) {
        _progression.HandleGateHit(ref dynState, Side.Left);
        _ballMovement.Freeze(ref dynState);
        dynState.SpeedFactor = SnMath.One;
      }
      else if (Collision2D.Check(dynState.Ball.ToCircle(_stState.BallSize), _stState.GateRight, SnVector2.Right)) {
        _progression.HandleGateHit(ref dynState, Side.Right);
        _ballMovement.Freeze(ref dynState);
        dynState.SpeedFactor = SnMath.One;
      }
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
  }
}