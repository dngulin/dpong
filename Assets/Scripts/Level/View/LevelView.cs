using System;
using DPong.Common;
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

    public LevelView(LevelState initialState, LevelSettings settings) {
      _viewRoot = new GameObject("LevelViewRoot").transform;

      var res = Resources.Load<LevelViewResources>("LevelViewResources");

      _board = UObj.Instantiate(res.Board, _viewRoot).Configured(settings.PlayerLeft, settings.PlayerRight);
      _ball = UObj.Instantiate(res.Ball, _viewRoot);
      _blockerLeft = UObj.Instantiate(res.Blocker, _viewRoot).Configured(Side.Left);
      _blockerRight = UObj.Instantiate(res.Blocker, _viewRoot).Configured(Side.Right);

      Resources.UnloadAsset(res);

      ApplyState(initialState);
    }

    public void Dispose() {
      if (_viewRoot != null)
        UObj.Destroy(_viewRoot.gameObject);
    }

    public void UpdateState(in LevelState l, in LevelState r, float t) {
      const float threshold = 0.001f;

      if (t < threshold) {
        ApplyState(l);
      } else if (t > 1f - threshold) {
        ApplyState(r);
      }
      else {
        if (t < 0.5f)
          BlendAndApplyState(l, r, t);
        else
          BlendAndApplyState(r, l, 1f - t);
      }
    }

    private void BlendAndApplyState(in LevelState near, in LevelState far, float t) {
      var lHp = near.HitPoints.Left;
      var rHp = near.HitPoints.Right;

      Vector2 ballPos;
      if (near.Ball.FreezeCooldown > 0 || far.Ball.FreezeCooldown > 0) {
        ballPos = near.Ball.Position.ToVector2();
      }
      else {
        ballPos = Vector2.Lerp(near.Ball.Position.ToVector2(), far.Ball.Position.ToVector2(), t);
      }

      var lPos = Vector2.Lerp(near.Blockers.LeftPosition.ToVector2(), far.Blockers.LeftPosition.ToVector2(), t);
      var rPos = Vector2.Lerp(near.Blockers.RightPosition.ToVector2(), far.Blockers.RightPosition.ToVector2(), t);

      ApplyState(lHp, rHp, ballPos, lPos, rPos);
    }

    private void ApplyState(in LevelState state) {
      var lHp = state.HitPoints.Left;
      var rHp = state.HitPoints.Right;

      var ballPos = state.Ball.Position.ToVector2();

      var lPos = state.Blockers.LeftPosition.ToVector2();
      var rPos = state.Blockers.RightPosition.ToVector2();

      ApplyState(lHp, rHp, ballPos, lPos, rPos);
    }

    // TODO: Create ViewState
    private void ApplyState(int lHp, int rHp, in Vector2 ballPos, in Vector2 lPos, in Vector2 rPos) {
      _board.SetHitPoints(lHp, rHp);
      _ball.SetPosition(ballPos);
      _blockerLeft.SetPosition(lPos);
      _blockerRight.SetPosition(rPos);
    }
  }
}
