using DPong.Common;
using DPong.Level.State;
using UnityEngine;

namespace DPong.Level.View {
  public abstract class StateViewer: MonoBehaviour {
    private IDynamicStateContainer _dynStateContainer;
    protected StaticLevelState StState;

    private float _tickDuration = 1.0f / 60;
    private float _displayedTickTime;

    public void Initialize(IDynamicStateContainer dynStateContainer, StaticLevelState staticState) {
      _dynStateContainer = dynStateContainer;
      _dynStateContainer.OnCurrentStateChanged += OnCurrentStateChanged;

      StState = staticState;
      _tickDuration = StState.TickDuration.Unscaled();
    }

    private void OnCurrentStateChanged() {
      _displayedTickTime = 0;
    }

    private void Update() {
      if (_dynStateContainer == null)
        return;

      if (_displayedTickTime >= _tickDuration)
        return;

      _displayedTickTime += Time.deltaTime;
      UpdateImpl(_dynStateContainer.Previous, _dynStateContainer.Current, _displayedTickTime / _tickDuration);
    }

    protected abstract void UpdateImpl(DynamicLevelState prev, DynamicLevelState curr, float factor);

    private void OnDestroy() {
      if (_dynStateContainer == null)
        return;

      _dynStateContainer.OnCurrentStateChanged -= OnCurrentStateChanged;
    }

    private void OnDrawGizmos() {
      DrawGizmos(_dynStateContainer.Current);
    }

    protected virtual void DrawGizmos(DynamicLevelState dynState) {
    }
  }
}