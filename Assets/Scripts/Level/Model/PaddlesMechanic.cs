using DPong.Level.Data;
using DPong.Level.State;
using FxNet.Math;

namespace DPong.Level.Model {
  public class PaddlesMechanic {
    private readonly FxNum _tickDuration;
    private readonly FxNum _speed;

    private readonly FxNum _maxPaddleDeviation;
    private readonly FxRectSize _paddleRect;

    public readonly PaddlesState InitialState;

    public PaddlesMechanic(PaddleSettings paddle, in FxRectSize boardSize, in FxNum tickDuration) {
      _tickDuration = tickDuration;
      _speed = paddle.Speed;

      _maxPaddleDeviation = (boardSize.Height - paddle.Size.Height) >> 1;
      _paddleRect = paddle.Size;

      var blockerPos = new FxVec2(boardSize.Width >> 1, 0);

      InitialState = new PaddlesState();
      InitialState.Left.Position = -blockerPos;
      InitialState.Right.Position = blockerPos;
    }

    public (BounceObj, BounceObj) Move(ref LevelState state, Keys leftKeys, Keys rightKeys) {
      var dl = Move(ref state.Paddles.Left.Position, state.Pace, leftKeys);
      var dr = Move(ref state.Paddles.Right.Position, state.Pace, rightKeys);

      return (
        GetPaddleBounceObj(state.Paddles.Left.Position, (FxVec2.Left * 20 - dl).Normalized()),
        GetPaddleBounceObj(state.Paddles.Right.Position, (FxVec2.Right * 20 - dr).Normalized())
        );
    }

    private FxVec2 Move(ref FxVec2 position, in FxNum pace, Keys keys) {
      if (keys == Keys.None || keys == (Keys.Up | Keys.Down))
        return FxVec2.Zero;

      var moveSign = keys.HasKey(Keys.Up) ? 1 : -1;
      var offsetMultiplier = _tickDuration * pace;
      var offset = _speed * moveSign * offsetMultiplier;

      var prevPosition = position;
      var newY = FxMath.Clamp(prevPosition.Y + offset, -_maxPaddleDeviation, _maxPaddleDeviation);
      position = new FxVec2(prevPosition.X, newY);

      return position - prevPosition;
    }

    private BounceObj GetPaddleBounceObj(in FxVec2 position, in FxVec2 inNormal) {
      return new BounceObj(_paddleRect.ToPolygon(position), inNormal);
    }
  }
}