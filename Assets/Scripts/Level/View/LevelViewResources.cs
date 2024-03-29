using UnityEngine;

namespace DPong.Level.View {
  [CreateAssetMenu(fileName = "LevelViewResources", menuName = "DPong/LevelViewResources")]
  public class LevelViewResources: ScriptableObject {
    public BoardView Board;
    public PaddleView Paddle;
    public BallView Ball;
  }
}