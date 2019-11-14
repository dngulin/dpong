using PGM.Collisions.Shapes2D;
using PGM.ScaledNum;
using UnityEngine;

namespace DPong.Common {
  public static class PgmBridge {
    public static float Unscaled(this long scaledNumber) => (float) scaledNumber / SnMath.Scale;
    public static Vector2 ToVector2(this SnVector2 v) => new Vector2(v.X.Unscaled(), v.Y.Unscaled());
    public static Vector2 ToVector2(this RectSize2D s) => new Vector2(s.Width.Unscaled(), s.Height.Unscaled());
  }
}