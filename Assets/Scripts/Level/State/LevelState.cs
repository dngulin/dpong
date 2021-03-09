using DPong.Common;
using FxNet.Math;
using FxNet.Random;

namespace DPong.Level.State {
  public struct LevelState {
    public FxNum Pace;
    public FxRandomState Random;
    public HitPointsState HitPoints;
    public BallState Ball;
    public BlockersState Blockers;

    public int CalculateCheckSum() {
      var hash = Pace.CalculateCheckSum();
      const int k = 397;

      hash = (hash * k) ^ Random.CalculateCheckSum();

      hash = (hash * k) ^ HitPoints.CalculateCheckSum();
      hash = (hash * k) ^ Ball.CalculateCheckSum();
      hash = (hash * k) ^ Blockers.CalculateCheckSum();

      return hash;
    }
  }
}