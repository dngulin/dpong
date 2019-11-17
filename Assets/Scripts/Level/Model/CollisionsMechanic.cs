using DPong.Level.Data;
using PGM.Collisions;
using PGM.Collisions.Shapes2D;
using PGM.ScaledNum;
using PGM.SceneGraph;

namespace DPong.Level.Model {
  public class CollisionsMechanic {
    public BounceObj BorderUp;
    public BounceObj BorderDown;

    private readonly ShapeState2D _lGateShape;
    private readonly ShapeState2D _rGateShape;

    public CollisionsMechanic(BoardSettings board) {
      var gatePos = new SnVector2((board.Size.Width + board.GateSize.Width) / 2, 0);

      _lGateShape = new ShapeState2D(ShapeType2D.Rect, new ShapeSize2D(board.GateSize), Transform.Translate(-gatePos));
      _rGateShape = new ShapeState2D(ShapeType2D.Rect, new ShapeSize2D(board.GateSize), Transform.Translate(gatePos));

      var borderPos = new SnVector2(0, (board.Size.Height + board.BorderSize.Height) / 2);

      var up = new ShapeState2D(ShapeType2D.Rect, new ShapeSize2D(board.BorderSize), Transform.Translate(-borderPos));
      var down = new ShapeState2D(ShapeType2D.Rect, new ShapeSize2D(board.BorderSize), Transform.Translate(borderPos));

      BorderUp = new BounceObj(up, SnVector2.Zero);
      BorderDown = new BounceObj(down, SnVector2.Zero);
    }

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
      var gates = side == Side.Left ? _lGateShape : _rGateShape;
      var direction = Shape2D.GetCenter(gates.Transform) - Shape2D.GetCenter(shape.Transform);
      return Collision2D.Check(shape, gates, direction);
    }
  }
}