using PGM.Collisions.Shapes2D;
using PGM.ScaledNum;

namespace DPong.Level.State {
  public struct ColliderState {
    public ShapeType2D Shape;
    public ShapeSize2D Size;
    public ShapePose2D Pose;
    public SnMatrix3 Transform;
  }

  public static class ColliderStateExtensions {
    public static ShapeState2D ToShapeState(this ColliderState state) {
      return new ShapeState2D(state.Shape, state.Size, state.Transform);
    }
  }
}