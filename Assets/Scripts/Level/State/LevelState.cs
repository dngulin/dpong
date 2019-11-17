using PGM.Random;

namespace DPong.Level.State {
  public struct LevelState {
    public PcgState Random;
    public long Pace;
    public HitPointsState HitPoints;
    public BallState Ball;
    public BlockersState Blockers;
  }
}