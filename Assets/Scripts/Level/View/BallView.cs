using DPong.Common;
using DPong.Level.State;
using UnityEngine;

namespace DPong.Level.View {
  public class BallView: StateViewer {
    protected override void UpdateImpl(LevelState previous, LevelState current, float factor) {
      var prevPos = previous.Ball.Pose.Position.ToUnityVector();
      var currPos = previous.Ball.Pose.Position.ToUnityVector();

      transform.localPosition = Vector3.Lerp(prevPos, currPos, factor);
    }
  }
}