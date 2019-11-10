using UnityEngine;

namespace DPong.Level.View {
  [CreateAssetMenu(fileName = "LevelViewResources", menuName = "DPONG/LevelViewResources")]
  public class LevelViewResources: ScriptableObject {
    public BoardView Board;
    public BlockerView Blocker;
    public BallView Ball;
  }
}