using DPong.Level.Data;
using PGM.Collisions.Shapes2D;
using PGM.ScaledNum;

namespace DPong.Level.State {
  public class StaticLevelState {
    public readonly int HitPoints = 5;

    public readonly long FreezeTime = 3_000;
    public readonly long SpeedFactorInc = 0_100;
    public readonly long SpeedFactorMax = 2_700;

    public readonly long BlockerSpeed = 7_000;
    public readonly long BallSpeed = 8_000;

    public readonly RectSize2D BoardSize = new RectSize2D(30_000, 20_000);
    public readonly RectSize2D MarginSize = new RectSize2D(70_000, 10_000);
    public readonly RectSize2D GateSize = new RectSize2D(10_000, 20_000);
    public readonly RectSize2D BlockerSize = new RectSize2D(1_250, 5_000);

    public readonly CircleSize2D BallSize= new CircleSize2D(0_500);

    public readonly PlayerInfo PlayerLeft;
    public readonly PlayerInfo PlayerRight;

    public readonly long TickDuration;

    public readonly ShapeState2D GateLeft;
    public readonly ShapeState2D GateRight;
    public readonly ShapeState2D MarginDown;
    public readonly ShapeState2D MarginUpper;

    public StaticLevelState(LevelInfo info) {
      PlayerLeft = info.Left;
      PlayerRight = info.Right;
      TickDuration = info.Settings.TickDuration;

      var gatePos = new SnVector2((BoardSize.Width + GateSize.Width) / 2, 0);
      GateLeft = GetRectShapeState(GateSize, -gatePos);
      GateRight = GetRectShapeState(GateSize, gatePos);

      var marginPos = new SnVector2(0, (BoardSize.Height + MarginSize.Height) / 2);
      MarginDown = GetRectShapeState(MarginSize, -marginPos);
      MarginUpper = GetRectShapeState(MarginSize, marginPos);
    }

    private static ShapeState2D GetRectShapeState(in RectSize2D size, in SnVector2 position) {
      var pose = new ShapePose2D {Position = position, Rotation = 0, Offset = SnVector2.Zero};
      return new ShapeState2D(ShapeType2D.Rect, new ShapeSize2D(size), Shape2D.GetTransform(pose));
    }
  }
}