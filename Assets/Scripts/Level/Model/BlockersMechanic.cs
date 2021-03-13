using DPong.Level.Data;
using DPong.Level.State;
using FxNet.Math;

namespace DPong.Level.Model {
  public class BlockersMechanic {
    private readonly FxNum _tickDuration;
    private readonly FxNum _speed;

    private readonly FxNum _maxBlockerDeviation;
    private readonly FxRectSize _blockerRect;

    public readonly BlockerState2 InitialState;

    public BlockersMechanic(BlockerSettings blocker, in FxRectSize boardSize, in FxNum tickDuration) {
      _tickDuration = tickDuration;
      _speed = blocker.Speed;

      _maxBlockerDeviation = (boardSize.Height - blocker.Size.Height) >> 1;
      _blockerRect = blocker.Size;

      var blockerPos = new FxVec2(boardSize.Width >> 1, 0);

      InitialState = new BlockerState2();
      InitialState[0].Position = -blockerPos;
      InitialState[1].Position = blockerPos;
    }

    public (BounceObj, BounceObj) Move(ref LevelState state, Keys leftKeys, Keys rightKeys) {
      Move(ref state.Blockers[0].Position, state.Pace, leftKeys);
      Move(ref state.Blockers[1].Position, state.Pace, rightKeys);

      var deltaL = new FxVec2(0, state.Blockers[0].Position.Y - state.Ball.Position.Y);
      var deltaR = new FxVec2(0, state.Blockers[1].Position.Y - state.Ball.Position.Y);

      return (
        GetBlockerBounceObj(state.Blockers[0].Position, (FxVec2.Left * 20 - deltaL).Normalized()),
        GetBlockerBounceObj(state.Blockers[1].Position, (FxVec2.Right * 20 - deltaR).Normalized())
        );
    }

    private void Move(ref FxVec2 position, in FxNum pace, Keys keys) {
      if (keys == Keys.None || keys == (Keys.Up | Keys.Down)) return;

      var moveSign = keys.HasKey(Keys.Up) ? 1 : -1;
      var offsetMultiplier = _tickDuration * pace;
      var offset = _speed * moveSign * offsetMultiplier;

      var prevPosition = position;
      var newY = FxMath.Clamp(prevPosition.Y + offset, -_maxBlockerDeviation, _maxBlockerDeviation);
      position = new FxVec2(prevPosition.X, newY);
    }

    private BounceObj GetBlockerBounceObj(in FxVec2 position, in FxVec2 inNormal) {
      return new BounceObj(_blockerRect.ToPolygon(position), inNormal);
    }
  }
}