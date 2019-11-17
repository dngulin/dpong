using DPong.Level.State;
using PGM.ScaledNum;

namespace DPong.Level.Model {
  public class PaceMechanic {
    private readonly StaticLevelState _stState;

    public PaceMechanic(StaticLevelState stState) {
      _stState = stState;
    }

    public long Default => SnMath.One;

    public long SpeedUp(long speedFactor) {
      return SnMath.Clamp(speedFactor + _stState.SpeedFactorInc, Default, _stState.SpeedFactorMax);
    }
  }
}