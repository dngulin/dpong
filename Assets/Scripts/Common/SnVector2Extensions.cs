using PGM.ScaledNum;

namespace DPong.Common {
  public static class SnVector2Extensions {
    public static SnVector3 To3D(this SnVector2 v) => new SnVector3(v.X, v.Y, 0);
  }
}