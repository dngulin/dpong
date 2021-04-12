using System;
using DPong.Assets;
using DPong.Level.Data;
using DPong.Level.State;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace DPong.Level.View {
  public class LevelView: IDisposable {
    private readonly Transform _viewRoot;

    private readonly BoardView _board;
    private readonly BallView _ball;
    private readonly BlockerView _blockerLeft;
    private readonly BlockerView _blockerRight;

    public LevelView(AssetLoader assetLoader, in LevelState initialState, LevelSettings settings) {
      _viewRoot = new GameObject("LevelViewRoot").transform;

      var res = assetLoader.Load<LevelViewResources>("Assets/Content/Level/LevelViewResources.asset");

      _board = UObj.Instantiate(res.Board, _viewRoot).Configured(settings.PlayerLeft, settings.PlayerRight);
      _ball = UObj.Instantiate(res.Ball, _viewRoot);
      _blockerLeft = UObj.Instantiate(res.Blocker, _viewRoot).Configured(Side.Left);
      _blockerRight = UObj.Instantiate(res.Blocker, _viewRoot).Configured(Side.Right);

      ApplyState(initialState.ToViewState());
    }

    public void Dispose() {
      if (_viewRoot != null)
        UObj.Destroy(_viewRoot.gameObject);
    }

    public void UpdateState(in LevelState l, in LevelState r, float t) {
      const float threshold = 0.001f;

      if (t < threshold) {
        ApplyState(l.ToViewState());
      } else if (t > 1f - threshold) {
        ApplyState(r.ToViewState());
      }
      else {
        var vl = l.ToViewState();
        var vr = r.ToViewState();

        if (t < 0.5f)
          BlendAndApplyState(ref vl, vr, t);
        else
          BlendAndApplyState(ref vr, vl, 1f - t);
      }
    }

    private void BlendAndApplyState(ref LevelViewState near, in LevelViewState far, float t) {
      if (near.Ball.FreezeCooldown <= 0 && far.Ball.FreezeCooldown <= 0)
        near.Ball.Position = Vector2.Lerp(near.Ball.Position, far.Ball.Position, t);

      near.Blockers[0].Position = Vector2.Lerp(near.Blockers[0].Position, far.Blockers[0].Position, t);
      near.Blockers[1].Position = Vector2.Lerp(near.Blockers[1].Position, far.Blockers[1].Position, t);

      ApplyState(near);
    }

    private void ApplyState(in LevelViewState state) {
      _board.SetScore(state.Scores.Left, state.Scores.Right);
      _ball.SetPosition(state.Ball.Position);
      _blockerLeft.SetPosition(state.Blockers[0].Position);
      _blockerRight.SetPosition(state.Blockers[1].Position);
    }
  }
}
