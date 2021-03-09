using FxNet.Collision2D;
using FxNet.Math;

namespace DPong.Level.Model {
  public readonly struct BounceObj {
    public readonly FxPoly4 Shape;
    public readonly FxVec2 BounceInNormal;

    public BounceObj(in FxPoly4 shape, in FxVec2 bounceInNormal) {
      Shape = shape;
      BounceInNormal = bounceInNormal;
    }
  }
}