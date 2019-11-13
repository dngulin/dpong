using PGM.Collisions.Shapes2D;
using PGM.ScaledNum;
using UnityEngine;

namespace DPong.Common {
  public static class PgmBridge {
    public static float Unscaled(this long scaledNumber) {
      return scaledNumber / (float) SnMath.One;
    }

    public static Vector2 ToVector2(this SnVector2 v) {
      var x = (float) v.X / SnMath.Scale;
      var y = (float) v.Y / SnMath.Scale;
      return new Vector2(x, y);
    }

    public static Vector2 ToVector2(this RectSize2D v) {
      var x = (float) v.Width / SnMath.Scale;
      var y = (float) v.Height / SnMath.Scale;
      return new Vector2(x, y);
    }
  }
}