using DPong.Level.Data;
using DPong.Level.State;
using PGM.Collisions;
using PGM.Collisions.Shapes2D;
using PGM.Random;
using PGM.ScaledNum;

namespace DPong.Level.Model {
  public class LevelModel {
    private readonly StaticLevelState _stState;
    private readonly PcgState _initialPcgState;

    private readonly ProgressionMechanic _progression;
    private readonly GamePaceMechanic _gamePace;
    private readonly BlockerControlMechanic _blockerControl;
    private readonly BallMovementMechanic _ballMovement;

    public LevelModel(StaticLevelState stState, PcgState? pcgState) {
      _stState = stState;
      _initialPcgState = pcgState ?? Pcg.CreateState();

      _progression = new ProgressionMechanic(stState);
      _gamePace = new GamePaceMechanic(stState);
      _blockerControl = new BlockerControlMechanic(stState);
      _ballMovement = new BallMovementMechanic(stState);
    }

    public DynamicLevelState CreateInitialState() {
      var blockerPos = new SnVector2(_stState.BoardSize.Width / 2, 0);

      return new DynamicLevelState {
        Random = _initialPcgState,
        SpeedFactor = _gamePace.DefaultSpeed,

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

      var lMove = _blockerControl.Move(ref dynState.LeftBlocker, leftKeys, dynState.SpeedFactor);
      var rMove = _blockerControl.Move(ref dynState.RightBlocker, rightKeys, dynState.SpeedFactor);

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
        dynState.SpeedFactor = _gamePace.DefaultSpeed;
      }
      else if (Collision2D.Check(dynState.Ball.ToCircle(_stState.BallSize), _stState.GateRight, SnVector2.Right)) {
        _progression.HandleGateHit(ref dynState, Side.Right);
        _ballMovement.Freeze(ref dynState);
        dynState.SpeedFactor = _gamePace.DefaultSpeed;
      }
    }

    private unsafe void CheckBounce(ref DynamicLevelState dynState, in ShapeState2D blocker) {
      var ball = dynState.Ball.ToCircle(_stState.BallSize);
      if (!Collision2D.Check(ball, blocker, dynState.BallSpeed))
        return;

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

      var shift = -penetrations[minIndex];
      _ballMovement.Shift(ref dynState, shift);
      _ballMovement.Bounce(ref dynState, shift.Normalized());

      dynState.SpeedFactor = _gamePace.SpeedUp(dynState.SpeedFactor);
    }
  }
}