using PGM.Collisions.Shapes2D;

namespace DPong.Level.Data {
  public class BallSettings {
    public readonly long FreezeTime = 3_000;

    public readonly long Speed = 8_000;
    public readonly CircleSize2D Size = new CircleSize2D(0_500);

    public readonly int BounceRandomAngle = 2_500;
    public readonly int BounceMovementAngle = 15_000;
  }
}