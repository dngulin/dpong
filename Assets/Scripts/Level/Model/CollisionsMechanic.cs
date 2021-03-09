using DPong.Level.Data;
using FxNet.Collision2D;
using FxNet.Math;

namespace DPong.Level.Model {
  public class CollisionsMechanic {
    public readonly BounceObj BorderUp;
    public readonly BounceObj BorderDown;

    private FxPoly4 _lGateShape;
    private FxPoly4 _rGateShape;

    public CollisionsMechanic(BoardSettings board) {
      var gatePos = new FxVec2((board.Size.Width + board.GateSize.Width) >> 1, 0);

      _lGateShape = board.GateSize.ToPolygon(-gatePos);
      _rGateShape = board.GateSize.ToPolygon(gatePos);

      var borderPos = new FxVec2(0, (board.Size.Height + board.BorderSize.Height) >> 1);

      var upShape = board.BorderSize.ToPolygon(borderPos);
      var dnShape = board.BorderSize.ToPolygon(-borderPos);

      BorderUp = new BounceObj(upShape, FxVec2.Up);
      BorderDown = new BounceObj(dnShape, FxVec2.Down);
    }

    public unsafe bool Check(in FxCircle a, in FxPoly4 b, out FxVec2 penetration) {
      penetration = FxVec2.Zero;

      if (!FxCollisionChecker2D.Check(a, b, a.Center - (b.A + b.C) >> 1))
        return false;

      const int pLen = 4;
      var penetrations = stackalloc FxVec2[pLen] {
        FxCollisionChecker2D.GetPenetration(a, b, FxVec2.Up),
        FxCollisionChecker2D.GetPenetration(a, b, FxVec2.Left),
        FxCollisionChecker2D.GetPenetration(a, b, FxVec2.Down),
        FxCollisionChecker2D.GetPenetration(a, b, FxVec2.Right)
      };

      var minSqMag = FxNum.FromRaw(long.MaxValue);
      var minIndex = 0;
      for (var i = 0; i < pLen; i++) {
        var curSqMag = penetrations[i].SqrMagnitude();
        if (curSqMag >= minSqMag)
          continue;

        minSqMag = curSqMag;
        minIndex = i;
      }

      penetration = penetrations[minIndex];
      return true;
    }

    public bool CheckGates(in FxCircle ball, Side side) {
      ref var gates = ref side == Side.Left ? ref _lGateShape : ref _rGateShape;
      return FxCollisionChecker2D.Check(ball, gates, FxVec2.Right);
    }
  }
}