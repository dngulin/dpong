using FxNet.Math;

namespace DPong.Level.Data {
  public class BallSettings {
    public readonly FxNum FreezeTime = FxNum.FromMillis(3_000);

    public readonly FxNum Speed = FxNum.FromMillis(8_000);
    public readonly FxNum Radius = FxNum.FromMillis(0_500);

    public readonly FxNum BounceRandomAngle = FxNum.FromMillis(1_500);
  }
}