using DPong.Level.Data;
using DPong.Level.State;

namespace DPong.Level.Model {
  public class ProgressMechanic {
    private readonly StaticLevelState _stState;

    public int DefaultHp => _stState.HitPoints;

    public ProgressMechanic(StaticLevelState stState) {
      _stState = stState;
    }

    public bool IsLevelCompleted(in LevelState state) {
      return state.LeftHp <= 0 || state.RightHp <= 0;
    }

    public void DecreaseHp(ref LevelState state, Side side) {
      if (IsLevelCompleted(state))
        return;

      ref var selectedHp = ref side == Side.Left ? ref state.LeftHp : ref state.RightHp;
      selectedHp--;
    }
  }
}