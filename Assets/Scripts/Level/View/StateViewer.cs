using DPong.Level.State;
using UnityEngine;

namespace DPong.Level.View {
  public abstract class StateViewer: MonoBehaviour {
    private IDisplayingStateHolder _stateHolder;
    private float _frameTimeTotal = 1.0f / 60;
    private float _frameTimeCurrent;

    public void Init(IDisplayingStateHolder stateHolder, float frameTime) {
      _stateHolder = stateHolder;
      _stateHolder.OnCurrentStateChanged += OnStateChanged;
      _frameTimeTotal = frameTime;
    }

    private void OnStateChanged() {
      _frameTimeCurrent = 0;
    }

    private void Update() {
      if (_stateHolder == null)
        return;

      if (_frameTimeCurrent >= _frameTimeTotal)
        return;

      _frameTimeCurrent = Mathf.Clamp(_frameTimeCurrent + Time.deltaTime, 0, _frameTimeTotal);
      UpdateImpl(_stateHolder.Previous, _stateHolder.Current, _frameTimeCurrent / _frameTimeTotal);
    }

    protected abstract void UpdateImpl(LevelState previous, LevelState current, float factor);

    private void OnDestroy() {
      if (_stateHolder == null)
        return;

      _stateHolder.OnCurrentStateChanged -= OnStateChanged;
    }
  }
}