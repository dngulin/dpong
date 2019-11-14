using DPong.Level.Data;
using DPong.Level.State;

namespace DPong.Level.Model {
  public class ProgressionMechanic {
    private readonly StaticLevelState _stState;

    public int DefaultHp => _stState.HitPoints;

    public ProgressionMechanic(StaticLevelState stState) {
      _stState = stState;
    }

    public bool IsLevelCompleted(in DynamicLevelState dynState) {
      return dynState.LeftHp <= 0 || dynState.RightHp <= 0;
    }

    public void HandleGateHit(ref DynamicLevelState dynState, Side side) {
      if (IsLevelCompleted(dynState))
        return;

      if (side == Side.Left) {
        dynState.LeftHp--;
      }
      else {
        dynState.RightHp--;
      }
    }
  }
}