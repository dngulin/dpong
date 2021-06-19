using FxNet.Math;

namespace DPong.Level.Data {
  public class PaddleSettings {
    public readonly FxNum Speed = FxNum.FromMillis(7_000);
    public readonly FxRectSize Size = new FxRectSize(FxNum.FromMillis(1_250), FxNum.FromMillis(5_000));
  }
}