using FxNet.Math;
using UnityEngine;

namespace DPong.Common {
  public static class FxUnityBridge {
    public static Vector2 ToVector2(this in FxVec2 v) => new Vector2((float) v.X, (float) v.Y);
  }
}