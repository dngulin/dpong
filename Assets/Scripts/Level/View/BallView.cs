using UnityEngine;

namespace DPong.Level.View {
  public class BallView: MonoBehaviour {
    public void SetPosition(in Vector2 pos) => transform.localPosition = pos;
  }
}