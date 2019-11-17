using DPong.Level.Data;
using PGM.ScaledNum;

namespace DPong.Level.Model {
  public class PaceMechanic {
    public readonly long Default;

    private readonly long _inc;
    private readonly long _max;

    public PaceMechanic(PaceSettings pace) {
      Default = pace.Default;
      _inc = pace.BounceInc;
      _max = pace.Maximum;
    }

    public long SpeedUp(long pace) => SnMath.Clamp(pace + _inc, Default, _max);
  }
}