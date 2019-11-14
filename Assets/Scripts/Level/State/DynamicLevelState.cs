using PGM.Random;
using PGM.ScaledNum;

namespace DPong.Level.State {
  public struct DynamicLevelState {
    public PcgState Random;

    public long SpeedFactor;

    public int LeftHp;
    public int RightHp;

    public long FreezeTime;
    public SnVector2 BallSpeed;

    public ColliderState Ball;
    public ColliderState LeftBlocker;
    public ColliderState RightBlocker;
  }
}