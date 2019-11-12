using DPong.Common;
using DPong.Level.State;
using UnityEngine;

namespace DPong.Level.View {
  public class BallView: StateViewer {
    protected override void UpdateImpl(LevelState prev, LevelState curr, float factor) {
      var prevPos = prev.Ball.Pose.Position.ToUnityVector();
      var currPos = curr.Ball.Pose.Position.ToUnityVector();

      transform.localPosition = Vector3.Lerp(prevPos, currPos, factor);
    }

    protected override void DrawGizmos(LevelState state) {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(state.Ball.Pose.Position.ToUnityVector(), 0.5f);
    }
  }
}