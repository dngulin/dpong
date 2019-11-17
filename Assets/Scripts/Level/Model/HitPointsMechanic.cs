using DPong.Level.Data;
using DPong.Level.State;

namespace DPong.Level.Model {
  public class HitPointsMechanic {
    public readonly HitPointsState InitialState;

    public HitPointsMechanic(int hitPoints) {
      InitialState = new HitPointsState {Left = hitPoints, Right = hitPoints};
    }

    public bool IsLevelCompleted(in HitPointsState hitPoints) {
      return hitPoints.Left <= 0 || hitPoints.Right <= 0;
    }

    public void DecreaseHp(ref HitPointsState hitPoints, Side side) {
      if (IsLevelCompleted(hitPoints))
        return;

      ref var selectedHp = ref side == Side.Left ? ref hitPoints.Left : ref hitPoints.Right;
      selectedHp--;
    }
  }
}