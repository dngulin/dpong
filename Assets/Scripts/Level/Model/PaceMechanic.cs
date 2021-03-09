using DPong.Level.Data;
using FxNet.Math;

namespace DPong.Level.Model {
  public class PaceMechanic {
    public readonly FxNum Default;

    private readonly FxNum _inc;
    private readonly FxNum _max;

    public PaceMechanic(PaceSettings pace) {
      Default = pace.Default;
      _inc = pace.BounceInc;
      _max = pace.Maximum;
    }

    public FxNum SpeedUp(in FxNum pace) => FxMath.Clamp(pace + _inc, Default, _max);
  }
}