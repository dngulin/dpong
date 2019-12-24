using PGM.ScaledNum;

namespace DPong.Level.State {
  public struct BallState {
    public long FreezeCooldown;
    public SnVector2 Speed;
    public SnVector2 Position;

    public int CalculateHash() {
      var hash = FreezeCooldown.GetHashCode();

      hash = (hash * 397) ^ Speed.GetHashCode();
      hash = (hash * 397) ^ Position.GetHashCode();

      return hash;
    }
  }
}