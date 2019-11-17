using DPong.Level.Data;
using DPong.Level.State;
using PGM.Collisions;
using PGM.Collisions.Shapes2D;
using PGM.ScaledNum;

namespace DPong.Level.Model {
  public class CollisionsMechanic {
    private readonly StaticLevelState _stState;

    public CollisionsMechanic(StaticLevelState stState) {
      _stState = stState;
    }

    public BounceObj BorderUp => new BounceObj(_stState.BorderUp, SnVector2.Zero);
    public BounceObj BorderDown => new BounceObj(_stState.BorderDown, SnVector2.Zero);

    public unsafe bool Check(in ShapeState2D a, in ShapeState2D b, out SnVector2 penetration) {
      penetration = SnVector2.Zero;

      if (!Collision2D.Check(a, b, Shape2D.GetCenter(b.Transform) - Shape2D.GetCenter(a.Transform)))
        return false;

      const int pLen = 4;
      var penetrations = stackalloc SnVector2[pLen] {
        Collision2D.GetPenetration(a, b, SnVector2.Up),
        Collision2D.GetPenetration(a, b, SnVector2.Left),
        Collision2D.GetPenetration(a, b, SnVector2.Down),
        Collision2D.GetPenetration(a, b, SnVector2.Right)
      };

      var minSqMag = long.MaxValue;
      var minIndex = 0;
      for (var i = 0; i < pLen; i++) {
        var curSqMag = penetrations[i].SquareMagnitude();
        if (curSqMag >= minSqMag)
          continue;

        minSqMag = curSqMag;
        minIndex = i;
      }

      penetration = penetrations[minIndex];
      return true;
    }

    public bool CheckGates(ShapeState2D shape, Side side) {
      var gates = side == Side.Left ? _stState.GateLeft : _stState.GateRight;
      var direction = Shape2D.GetCenter(gates.Transform) - Shape2D.GetCenter(shape.Transform);
      return Collision2D.Check(shape, gates, direction);
    }
  }
}