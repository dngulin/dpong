using PGM.Collisions.Shapes2D;
using PGM.ScaledNum;

namespace DPong.Level.Model {
  public readonly struct BounceObj {
    public readonly ShapeState2D State;
    public readonly SnVector2 Movement;

    public BounceObj(in ShapeState2D state, in SnVector2 movement) {
      State = state;
      Movement = movement;
    }
  }
}