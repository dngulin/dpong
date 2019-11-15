using DPong.Level.State;
using PGM.ScaledNum;

namespace DPong.Level.Model {
  public class GamePaceMechanic {
    private readonly StaticLevelState _stState;

    public GamePaceMechanic(StaticLevelState stState) {
      _stState = stState;
    }

    public long DefaultSpeed => SnMath.One;

    public long SpeedUp(long speedFactor) {
      return SnMath.Clamp(speedFactor + _stState.SpeedFactorInc, DefaultSpeed, _stState.SpeedFactorMax);
    }
  }
}