using DPong.Level.State;
using PGM.Collisions.Shapes2D;
using PGM.ScaledNum;
using PGM.SceneGraph;

namespace DPong.Level.Model {
  public class BlockersMechanic {
    private readonly StaticLevelState _stState;
    private readonly long _maxBlockerDeviation;
    private readonly ShapeSize2D _size;

    public readonly BlockersState InitialState;

    public BlockersMechanic(StaticLevelState stState) {
      _stState = stState;
      _maxBlockerDeviation = (stState.BoardSize.Height - stState.BlockerSize.Height) / 2;
      _size = new ShapeSize2D(stState.BlockerSize);

      var blockerPos = new SnVector2(_stState.BoardSize.Width / 2, 0);
      InitialState = new BlockersState {
        LeftPosition = -blockerPos,
        RightPosition = blockerPos
      };
    }

    public (BounceObj, BounceObj) Move(ref BlockersState blockers, long pace, Keys leftKeys, Keys rightKeys) {
      var lBlocker = Move(ref blockers.LeftPosition, pace, leftKeys);
      var rBlocker = Move(ref blockers.RightPosition, pace, rightKeys);
      return (lBlocker, rBlocker);
    }

    private BounceObj Move(ref SnVector2 position, long pace, Keys keys) {
      if (keys == Keys.None || keys == (Keys.Up | Keys.Down))
        return GetBlockerBounceObject(position, SnVector2.Zero);

      var moveSign = keys.HasKey(Keys.Up) ? 1 : -1;

      var directedSpeed = _stState.BlockerSpeed * moveSign;
      var offsetMultiplier = SnMath.Mul(_stState.TickDuration, pace);
      var offset = SnMath.Mul(directedSpeed, offsetMultiplier);

      var prevPosition = position;
      var newY = SnMath.Clamp(prevPosition.Y + offset, -_maxBlockerDeviation, _maxBlockerDeviation);
      position = new SnVector2(prevPosition.X, newY);

      return GetBlockerBounceObject(position, position - prevPosition);
    }

    private BounceObj GetBlockerBounceObject(in SnVector2 position, in SnVector2 movement) {
      var shapeState = new ShapeState2D(ShapeType2D.Rect, _size, Transform.Translate(position));
      return new BounceObj(shapeState, movement);
    }
  }
}