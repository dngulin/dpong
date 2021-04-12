using System;
using DPong.Level.Data;
using DPong.Level.State;

namespace DPong.Level.Model {
  public class ScoreMechanic {
    private readonly int _targetScore;

    public ScoresState InitialState => new ScoresState {Left = 0, Right = 0};

    public ScoreMechanic(int targetScore) {
      _targetScore = targetScore;
    }

    public bool IsLevelCompleted(in ScoresState hitPoints) {
      return hitPoints.Left >= _targetScore || hitPoints.Right >= _targetScore;
    }

    public void HandleGoal(ref ScoresState hitPoints, Side gatesSide) {
      if (IsLevelCompleted(hitPoints))
        return;

      switch (gatesSide) {
        case Side.Left:
          hitPoints.Right++;
          break;
        case Side.Right:
          hitPoints.Left++;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(gatesSide), gatesSide, null);
      }
    }
  }
}