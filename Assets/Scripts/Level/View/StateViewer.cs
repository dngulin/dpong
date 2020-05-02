using DPong.Common;
using DPong.Level.Data;
using DPong.Level.State;
using UnityEngine;

namespace DPong.Level.View {
  public abstract class StateViewer: MonoBehaviour {
    private StateContainer _stateContainer;
    protected LevelSettings Settings;

    private float _tickDuration = 1.0f / 60;
    private float _displayedTickTime;

    public void Init(StateContainer stateContainer, LevelSettings settings) {
      _stateContainer = stateContainer;
      _stateContainer.OnCurrentStateChanged += OnCurrentStateChanged;

      Settings = settings;
      _tickDuration = Settings.Simulation.TickDuration.Unscaled();

      InitImpl(_stateContainer.Current);
    }

    protected virtual void InitImpl(LevelState state) {
    }

    private void OnDestroy() {
      if (_stateContainer == null)
        return;

      _stateContainer.OnCurrentStateChanged -= OnCurrentStateChanged;
    }

    private void OnCurrentStateChanged() {
      _displayedTickTime = 0;
    }

    private void Update() {
      if (_stateContainer == null)
        return;

      if (_displayedTickTime >= _tickDuration)
        return;

      _displayedTickTime += Time.deltaTime;
      UpdateImpl(_stateContainer.Previous, _stateContainer.Current, _displayedTickTime / _tickDuration);
    }

    protected abstract void UpdateImpl(LevelState prev, LevelState curr, float factor);

    private void OnDrawGizmos() {
      if (_stateContainer == null)
        return;

      DrawGizmosImpl(_stateContainer.Current);
    }

    protected virtual void DrawGizmosImpl(LevelState state) {
    }
  }
}