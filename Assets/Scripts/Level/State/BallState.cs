using DPong.Common;
using FxNet.Math;

namespace DPong.Level.State {
  public struct BallState {
    public FxNum FreezeCooldown;
    public FxVec2 Speed;
    public FxVec2 Position;

    public int CalculateCheckSum() {
      var hash = FreezeCooldown.CalculateCheckSum();

      hash = (hash * 397) ^ Speed.CalculateCheckSum();
      hash = (hash * 397) ^ Position.CalculateCheckSum();

      return hash;
    }
  }
}