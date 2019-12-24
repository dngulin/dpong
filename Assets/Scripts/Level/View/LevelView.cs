using System;
using System.Text;
using DPong.Level.Data;
using DPong.Level.State;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace DPong.Level.View {
  public class LevelView: IDisposable {
    public DynamicStateContainer StateContainer { get; }
    private readonly Transform _viewRoot;

    public LevelView(LevelState state, LevelSettings settings) {
      _viewRoot = new GameObject("LevelViewRoot").transform;
      StateContainer = new DynamicStateContainer(state);

      var res = Resources.Load<LevelViewResources>("LevelViewResources");

      var stateViewers = new StateViewer[] {
        UObject.Instantiate(res.Board, _viewRoot),
        UObject.Instantiate(res.Ball, _viewRoot),
        UObject.Instantiate(res.Blocker, _viewRoot).ConfiguredForSide(Side.Left),
        UObject.Instantiate(res.Blocker, _viewRoot).ConfiguredForSide(Side.Right)
      };

      foreach (var stateViewer in stateViewers)
        stateViewer.Init(StateContainer, settings);

      Resources.UnloadAsset(res);
    }

    public void ShowSessionFinished(uint[] frames, int[] hashes) {
      var sb = new StringBuilder();
      sb.Append("Session finished:");

      sb.AppendLine();
      sb.Append("Frames:");
      foreach (var frame in frames) sb.Append(" ").Append(frame);

      sb.AppendLine();
      sb.Append("Hashes:");
      foreach (var hash in hashes) sb.Append(" ").Append(hash);


      Debug.Log(sb.ToString());
    }

    public void ShowSessionClosed(string message) {
      Debug.LogError(message);
    }

    public void Dispose() {
      UObject.Destroy(_viewRoot);
    }
  }
}
