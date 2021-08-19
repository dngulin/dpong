using System;
using DPong.Assets;
using DPong.Level.Data;
using DPong.Level.State;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace DPong.Level.View {
  public class LevelView : IDisposable {
    private readonly Transform _viewRoot;

    private readonly BoardView _board;
    private readonly BallView _ball;
    private readonly PaddleView _paddleLeft;
    private readonly PaddleView _paddleRight;

#if UNITY_EDITOR
    private readonly LevelViewGizmoDrawer _gizmoDrawer;
#endif

    public LevelView(AssetLoader assetLoader, in LevelState initialState, LevelSettings settings) {
      _viewRoot = new GameObject("LevelViewRoot").transform;

      var res = assetLoader.Load<LevelViewResources>("Assets/Content/Level/LevelViewResources.asset");

      _board = UObj.Instantiate(res.Board, _viewRoot).Configured(settings.PlayerLeft, settings.PlayerRight);
      _ball = UObj.Instantiate(res.Ball, _viewRoot);
      _paddleLeft = UObj.Instantiate(res.Paddle, _viewRoot).Configured(Side.Left);
      _paddleRight = UObj.Instantiate(res.Paddle, _viewRoot).Configured(Side.Right);

      var viewState = initialState.ToViewState();

#if UNITY_EDITOR
      _gizmoDrawer = _viewRoot.gameObject.AddComponent<LevelViewGizmoDrawer>();
      _gizmoDrawer.Init(settings);
#endif

      ApplyState(viewState, false);
    }

    public void Dispose() {
      if (_viewRoot != null)
        UObj.Destroy(_viewRoot.gameObject);
    }

    public void UpdateState(in LevelState l, in LevelState r, float t) {
      const float threshold = 0.001f;

      if (t < threshold) {
        ApplyState(l.ToViewState(), false);
      }
      else if (t > 1f - threshold) {
        ApplyState(r.ToViewState(), false);
      }
      else {
        var vl = l.ToViewState();
        var vr = r.ToViewState();

#if UNITY_EDITOR
        _gizmoDrawer.SetInterpolationSources(vl, vr);
#endif

        if (t < 0.5f)
          BlendAndApplyState(ref vl, vr, t);
        else
          BlendAndApplyState(ref vr, vl, 1f - t);
      }
    }

    private void BlendAndApplyState(ref LevelViewState near, in LevelViewState far, float t) {
      if (near.Ball.FreezeCooldown <= 0 && far.Ball.FreezeCooldown <= 0)
        near.Ball.Position = Vector2.Lerp(near.Ball.Position, far.Ball.Position, t);

      near.Paddles.Left.Position = Vector2.Lerp(near.Paddles.Left.Position, far.Paddles.Left.Position, t);
      near.Paddles.Right.Position = Vector2.Lerp(near.Paddles.Right.Position, far.Paddles.Right.Position, t);

      ApplyState(near, true);
    }

    private void ApplyState(in LevelViewState state, bool interpolated) {
      _board.SetScore(state.Scores.Left, state.Scores.Right);
      _ball.SetPosition(state.Ball.Position);
      _paddleLeft.SetPosition(state.Paddles.Left.Position);
      _paddleRight.SetPosition(state.Paddles.Right.Position);

#if UNITY_EDITOR
      if (!interpolated)
        _gizmoDrawer.SetInterpolationSources(state, state);

      _gizmoDrawer.SetInterpolationResult(state);
#endif
    }
  }
}