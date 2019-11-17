using PGM.ScaledNum;

namespace DPong.Level.Data {
  public class PaceSettings {
    public readonly long Default = SnMath.One;
    public readonly long Maximum = 2_700;
    public readonly long BounceInc = 0_100;
  }
}