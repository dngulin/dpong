using PGM.Collisions.Shapes2D;
using PGM.ScaledNum;

namespace DPong.Level.State {
  public struct ColliderState {
    public SnVector2 Position;
    public SnMatrix3 Transform;
  }

  public static class ColliderStateExtensions {
    public static ShapeState2D ToRect(this ColliderState state, RectSize2D size) {
      return new ShapeState2D(ShapeType2D.Rect, new ShapeSize2D(size), state.Transform);
    }

    public static ShapeState2D ToCircle(this ColliderState state, CircleSize2D size) {
      return new ShapeState2D(ShapeType2D.Circle, new ShapeSize2D(size), state.Transform);
    }
  }
}