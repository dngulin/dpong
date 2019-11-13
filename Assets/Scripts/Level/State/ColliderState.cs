using PGM.Collisions.Shapes2D;
using PGM.ScaledNum;

namespace DPong.Level.State {
  public struct ColliderState {
    private SnVector2 _position;
    private SnMatrix3 _transform;

    public SnVector2 Position {
      get => _position;
      set {
        _position = value;
        _transform = GetTransform(value);
      }
    }

    public ColliderState(SnVector2 position) {
      _position = position;
      _transform = GetTransform(position);
    }

    private static SnMatrix3 GetTransform(SnVector2 position) {
      return Shape2D.GetTransform(new ShapePose2D {Position = position});
    }

    public ShapeState2D ToRect(RectSize2D size) {
      return new ShapeState2D(ShapeType2D.Rect, new ShapeSize2D(size), _transform);
    }

    public ShapeState2D ToCircle(CircleSize2D size) {
      return new ShapeState2D(ShapeType2D.Circle, new ShapeSize2D(size), _transform);
    }
  }
}