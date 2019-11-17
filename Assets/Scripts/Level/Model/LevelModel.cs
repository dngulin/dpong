using DPong.Level.Data;
using DPong.Level.State;
using PGM.Random;
using PGM.ScaledNum;

namespace DPong.Level.Model {
  public class LevelModel {
    private static readonly Side[] Sides = {Side.Left, Side.Right};

    private readonly StaticLevelState _stState;
    private readonly PcgState _initialPcgState;

    private readonly ProgressionMechanic _progression;
    private readonly GamePaceMechanic _gamePace;
    private readonly BlockerControlMechanic _blockerControl;
    private readonly BallMovementMechanic _ballMovement;
    private readonly CollisionsMechanic _collisions;

    public LevelModel(StaticLevelState stState, PcgState? pcgState) {
      _stState = stState;
      _initialPcgState = pcgState ?? Pcg.CreateState();

      _progression = new ProgressionMechanic(stState);
      _gamePace = new GamePaceMechanic(stState);
      _blockerControl = new BlockerControlMechanic(stState);
      _ballMovement = new BallMovementMechanic(stState);
      _collisions = new CollisionsMechanic(stState);
    }

    public LevelState CreateInitialState() {
      var blockerPos = new SnVector2(_stState.BoardSize.Width / 2, 0);

      return new LevelState {
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

    public unsafe void Tick(ref LevelState state, Keys leftKeys, Keys rightKeys) {
      if (_progression.IsLevelCompleted(state))
        return;

      var lBlocker = _blockerControl.Move(ref state.LeftBlocker, leftKeys, state.SpeedFactor);
      var rBlocker = _blockerControl.Move(ref state.RightBlocker, rightKeys, state.SpeedFactor);

      if (!_ballMovement.TryMove(ref state))
        return;

      const int bLen = 4;
      var objects = stackalloc BounceObj[bLen] {lBlocker, rBlocker, _collisions.BorderUp, _collisions.BorderDown};

      for (var i = 0; i < bLen; i++) {
        var obj = objects[i];
        if (!_collisions.Check(state.Ball.ToCircle(_stState.BallSize), obj.State, out var penetration))
          continue;

        _ballMovement.Shift(ref state, -penetration);
        _ballMovement.Bounce(ref state, -penetration.Normalized(), obj.Movement.Normalized());
        state.SpeedFactor = _gamePace.SpeedUp(state.SpeedFactor);
      }

      foreach (var side in Sides) {
        if (!_collisions.CheckGates(state.Ball.ToCircle(_stState.BallSize), side))
          continue;

        _progression.DecreaseHp(ref state, side);
        _ballMovement.Freeze(ref state);
        state.SpeedFactor = _gamePace.DefaultSpeed;
        break;
      }
    }
  }
}