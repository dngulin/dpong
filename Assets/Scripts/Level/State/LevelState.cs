using PGM.Random;

namespace DPong.Level.State {
  public struct LevelState {
    public long Pace;
    public PcgState Random;
    public HitPointsState HitPoints;
    public BallState Ball;
    public BlockersState Blockers;

    public int CalculateHash() {
      var hash = Pace.GetHashCode();
      const int k = 397;

      hash = (hash * k) ^ Random.Current.GetHashCode();
      hash = (hash * k) ^ Random.Sequence.GetHashCode();

      hash = (hash * k) ^ HitPoints.CalculateHash();
      hash = (hash * k) ^ Ball.CalculateHash();
      hash = (hash * k) ^ Blockers.CalculateHash();

      return hash;
    }
  }
}