using FxNet.Collision2D;
using FxNet.Math;

namespace DPong.Level.Data {
  public readonly struct FxRectSize {
    public readonly FxNum Width;
    public readonly FxNum Height;

    public FxRectSize(in FxNum width, in FxNum height) {
      Width = width;
      Height = height;
    }

    public FxPoly4 ToPolygon(in FxVec2 center) {
      var x = Width >> 1;
      var y = Height >> 1;
      return new FxPoly4(
        center + new FxVec2(-x, -y),
        center + new FxVec2(-x, y),
        center + new FxVec2(x, y),
        center + new FxVec2(x, -y));
    }
  }
}