using System;
using DPong.Level.Data;
using DPong.Level.State;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace DPong.Level.View {
  public class LevelView: IDisposable {
    public DynamicStateContainer DynStateContainer { get; }
    private readonly Transform _viewRoot;

    public LevelView(DynamicLevelState initialState, StaticLevelState stState) {
      _viewRoot = new GameObject("LevelViewRoot").transform;
      DynStateContainer = new DynamicStateContainer(initialState);

      var res = Resources.Load<LevelViewResources>("LevelViewResources");

      var stateViewers = new StateViewer[] {
        UObject.Instantiate(res.Board, _viewRoot).ConfiguredForPlayers(stState.PlayerLeft, stState.PlayerRight),
        UObject.Instantiate(res.Blocker, _viewRoot).ConfiguredForSide(Side.Left),
        UObject.Instantiate(res.Blocker, _viewRoot).ConfiguredForSide(Side.Right),
        UObject.Instantiate(res.Ball, _viewRoot),
      };

      foreach (var stateViewer in stateViewers)
        stateViewer.Initialize(DynStateContainer, stState);
    }

    public void Dispose() {
      UObject.Destroy(_viewRoot);
    }
  }
}
