using PGM.Random;
using PGM.ScaledNum;

namespace DPong.Level.State {
  public struct LevelState {
    public PcgState Random;
    public long Pace;

    public HitPointsState HitPoints;

    public BallState Ball;

    public ColliderState LeftBlocker;
    public ColliderState RightBlocker;
  }
}