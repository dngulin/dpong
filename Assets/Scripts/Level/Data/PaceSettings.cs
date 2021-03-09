using FxNet.Math;

namespace DPong.Level.Data {
  public class PaceSettings {
    public readonly FxNum Default = FxNum.FromMillis(1_000);
    public readonly FxNum Maximum = FxNum.FromMillis(2_700);
    public readonly FxNum BounceInc = FxNum.FromMillis(0_100);
  }
}