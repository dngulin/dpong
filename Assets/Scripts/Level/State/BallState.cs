using PGM.ScaledNum;

namespace DPong.Level.State {
  public struct BallState {
    public long FreezeCooldown;
    public SnVector2 Speed;
    public SnVector2 Position;
  }
}