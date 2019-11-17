using DPong.Level.State;
using PGM.ScaledNum;

namespace DPong.Level.Model {
  public class BlockersMechanic {
    private readonly StaticLevelState _stState;
    private readonly long _maxBlockerDeviation;

    public BlockersMechanic(StaticLevelState stState) {
      _stState = stState;
      _maxBlockerDeviation = (stState.BoardSize.Height - stState.BlockerSize.Height) / 2;
    }

    public BounceObj Move(ref ColliderState blocker, Keys keys, long speedFactor) {
      if (keys == Keys.None || keys == (Keys.Up | Keys.Down))
        return new BounceObj(blocker.ToRect(_stState.BlockerSize), SnVector2.Zero);

      var moveSign = keys.HasKey(Keys.Up) ? 1 : -1;

      var directedSpeed = _stState.BlockerSpeed * moveSign;
      var offsetMultiplier = SnMath.Mul(_stState.TickDuration, speedFactor);
      var offset = SnMath.Mul(directedSpeed, offsetMultiplier);

      var oldPos = blocker.Position;
      var newY = SnMath.Clamp(oldPos.Y + offset, -_maxBlockerDeviation, _maxBlockerDeviation);
      blocker.Position = new SnVector2(oldPos.X, newY);

      return new BounceObj(blocker.ToRect(_stState.BlockerSize), blocker.Position - oldPos);
    }
  }
}