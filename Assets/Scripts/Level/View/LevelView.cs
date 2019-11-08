using System;
using DPong.Level.Data;
using DPong.Level.State;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace DPong.Level.View {
  public class LevelView: IDisposable {
    public DisplayingStateHolder StateHolder { get; }
    private readonly Transform _viewRoot;

    public LevelView(LevelState initialState, float frameTime, PlayerInfo left, PlayerInfo right) {
      _viewRoot = new GameObject("LevelViewRoot").transform;
      StateHolder = new DisplayingStateHolder(initialState);

      var res = Resources.Load<LevelViewResources>("LevelResources");

      var stateViewers = new StateViewer[] {
        UObject.Instantiate(res.Board, _viewRoot).ConfiguredForPlayers(left, right),
        UObject.Instantiate(res.Blocker, _viewRoot).ConfiguredForSide(Side.Left),
        UObject.Instantiate(res.Blocker, _viewRoot).ConfiguredForSide(Side.Right),
        UObject.Instantiate(res.Ball, _viewRoot),
      };

      foreach (var stateViewer in stateViewers)
        stateViewer.Initialize(StateHolder, frameTime);
    }

    public void Dispose() {
      UObject.Destroy(_viewRoot);
    }
  }
}
