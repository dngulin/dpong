using DPong.Level.State;
using FxNet.Math;

namespace DPong.Level.AI {
  public class AiInputSource {
    public Keys GetLeft(LevelState state) => GetKeys(state.Blockers[0].Position, state.Ball.Position);

    public Keys GetRight(LevelState state) => GetKeys(state.Blockers[1].Position, state.Ball.Position);

    private Keys GetKeys(in FxVec2 blockerPos, in FxVec2 ballPos) {
      var delta = ballPos - blockerPos;

      if (FxMath.Abs(delta.X) < FxNum.FromMillis(1_250))
        return blockerPos.Y < ballPos.Y ? Keys.Up : Keys.Down;

      if (FxMath.Abs(delta.Y) < delta.Magnitude() >> 1)
        return Keys.None;

      return blockerPos.Y < ballPos.Y ? Keys.Up : Keys.Down;
    }
  }
}