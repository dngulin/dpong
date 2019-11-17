using DPong.Level.Data;
using DPong.Level.State;
using PGM.Collisions.Shapes2D;
using PGM.ScaledNum;
using PGM.SceneGraph;

namespace DPong.Level.Model {
  public class BlockersMechanic {
    private readonly long _tickDuration;
    private readonly long _speed;

    private readonly long _maxBlockerDeviation;
    private readonly ShapeSize2D _size;

    public readonly BlockersState InitialState;

    public BlockersMechanic(BlockerSettings blocker, in RectSize2D boardSize, long tickDuration) {
      _tickDuration = tickDuration;
      _speed = blocker.Speed;

      _maxBlockerDeviation = (boardSize.Height - blocker.Size.Height) / 2;
      _size = new ShapeSize2D(blocker.Size);

      var blockerPos = new SnVector2(boardSize.Width / 2, 0);
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
      var offsetMultiplier = SnMath.Mul(_tickDuration, pace);
      var offset = SnMath.Mul(_speed * moveSign, offsetMultiplier);

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