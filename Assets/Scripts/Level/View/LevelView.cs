using System;
using System.Text;
using DPong.Level.Data;
using DPong.Level.State;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace DPong.Level.View {
  public class LevelView: IDisposable {
    public StateContainer StateContainer { get; }
    private readonly Transform _viewRoot;

    public LevelView(LevelState state, LevelSettings settings) {
      _viewRoot = new GameObject("LevelViewRoot").transform;
      StateContainer = new StateContainer(state);

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

    public void Dispose() {
      if (_viewRoot.gameObject != null)
        UObject.Destroy(_viewRoot.gameObject);
    }
  }
}
