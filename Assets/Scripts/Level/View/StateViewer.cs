using DPong.Level.State;
using UnityEngine;

namespace DPong.Level.View {
  public abstract class StateViewer: MonoBehaviour {
    private IDisplayingStateHolder _stateHolder;
    private float _tickDuration = 1.0f / 60;
    private float _displayedTickTime;

    public void Initialize(IDisplayingStateHolder stateHolder, float tickDuration) {
      _stateHolder = stateHolder;
      _stateHolder.OnCurrentStateChanged += OnStateChanged;
      _tickDuration = tickDuration;
    }

    private void OnStateChanged() {
      _displayedTickTime = 0;
    }

    private void Update() {
      if (_stateHolder == null)
        return;

      if (_displayedTickTime >= _tickDuration)
        return;

      _displayedTickTime += Time.deltaTime;
      UpdateImpl(_stateHolder.Previous, _stateHolder.Current, _displayedTickTime / _tickDuration);
    }

    protected abstract void UpdateImpl(LevelState prev, LevelState curr, float factor);

    private void OnDestroy() {
      if (_stateHolder == null)
        return;

      _stateHolder.OnCurrentStateChanged -= OnStateChanged;
    }

    private void OnDrawGizmos() {
      DrawGizmos(_stateHolder.Current);
    }

    protected virtual void DrawGizmos(LevelState state) {
    }
  }
}