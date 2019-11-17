using DPong.Common;
using DPong.Level.State;
using UnityEngine;

namespace DPong.Level.View {
  public class BallView: StateViewer {
    protected override void UpdateImpl(LevelState prev, LevelState curr, float factor) {
      if (curr.Ball.FreezeCooldown > 0) {
        transform.localPosition = curr.Ball.Position.ToVector2();
        return;
      }

      var prevPos = prev.Ball.Position.ToVector2();
      var currPos = curr.Ball.Position.ToVector2();

      transform.localPosition = Vector3.Lerp(prevPos, currPos, factor);
    }

    protected override void DrawGizmos(LevelState state) {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(state.Ball.Position.ToVector2(), Settings.Ball.Size.Radius.Unscaled());
    }
  }
}