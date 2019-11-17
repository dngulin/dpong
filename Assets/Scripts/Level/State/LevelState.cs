using PGM.Random;
using PGM.ScaledNum;

namespace DPong.Level.State {
  public struct LevelState {
    public PcgState Random;
    public long SpeedFactor;

    public HitPointsState HitPoints;

    public long FreezeTime;
    public SnVector2 BallSpeed;

    public ColliderState Ball;
    public ColliderState LeftBlocker;
    public ColliderState RightBlocker;
  }
}