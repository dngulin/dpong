using PGM.Collisions.Shapes2D;

namespace DPong.Level.Data {
  public class BoardSettings {
    public readonly RectSize2D Size = new RectSize2D(30_000, 20_000);
    public readonly RectSize2D GateSize = new RectSize2D(10_000, 20_000);
    public readonly RectSize2D BorderSize = new RectSize2D(70_000, 10_000);
  }
}