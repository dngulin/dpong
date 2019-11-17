using System;
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
        UObject.Instantiate(res.Board, _viewRoot).ConfiguredForPlayers(settings.PlayerLeft, settings.PlayerRight),
        UObject.Instantiate(res.Blocker, _viewRoot).ConfiguredForSide(Side.Left),
        UObject.Instantiate(res.Blocker, _viewRoot).ConfiguredForSide(Side.Right),
        UObject.Instantiate(res.Ball, _viewRoot),
      };

      foreach (var stateViewer in stateViewers)
        stateViewer.Initialize(StateContainer, settings);
    }

    public void Dispose() {
      UObject.Destroy(_viewRoot);
    }
  }
}
