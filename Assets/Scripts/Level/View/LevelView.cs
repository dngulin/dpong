using System;
using DPong.Level.Data;
using DPong.Level.State;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace DPong.Level.View {
  public class LevelView: IDisposable {
    public DisplayingStateHolder StateHolder { get; }
    private readonly Transform _viewRoot;

    public LevelView(LevelState initialState, float frameTime, PlayerInfo left, PlayerInfo right) {
      _viewRoot = new GameObject("LevelViewRoot").transform;
      StateHolder = new DisplayingStateHolder(initialState);

      var resources = Resources.Load<LevelViewResources>("LevelResources");

      var stateViewers = new StateViewer[] {
        UnityObject.Instantiate(resources.BallView, _viewRoot)
      };

      foreach (var stateViewer in stateViewers)
        stateViewer.Init(StateHolder, frameTime);
    }

    public void Dispose() {
      UnityObject.Destroy(_viewRoot);
    }
  }
}
