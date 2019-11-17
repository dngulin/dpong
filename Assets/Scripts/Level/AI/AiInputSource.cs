using System;
using DPong.Level.State;
using PGM.ScaledNum;

namespace DPong.Level.AI {
  public class AiInputSource {
    public Keys GetLeft(LevelState state) => GetKeys(state.Blockers.LeftPosition, state.Ball.Position);

    public Keys GetRight(LevelState state) => GetKeys(state.Blockers.RightPosition, state.Ball.Position);

    private Keys GetKeys(in SnVector2 blockerPos, in SnVector2 ballPos) {
      var delta = ballPos - blockerPos;

      if (Math.Abs(delta.X) < 1_250)
        return blockerPos.Y < ballPos.Y ? Keys.Up : Keys.Down;

      if (Math.Abs(delta.Y) < delta.Magnitude() / 2)
        return Keys.None;

      return blockerPos.Y < ballPos.Y ? Keys.Up : Keys.Down;
    }
  }
}