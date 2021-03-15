using FxNet.Math;
using UnityEngine;

namespace DPong.Level.View {
  public static class FixedToFloatExtensions {
    public static float ToFloat(this in FxNum n) => (float) n;
    public static Vector2 ToVector2(this in FxVec2 v) => new Vector2((float) v.X, (float) v.Y);
  }
}