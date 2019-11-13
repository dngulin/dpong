using System;
using DPong.Level.State;

namespace DPong.Level.View {
  public class DynamicStateContainer : IDynamicStateContainer {
    public event Action OnCurrentStateChanged;

    public DynamicLevelState Current { private set; get; }
    public DynamicLevelState Previous { private set; get; }

    public DynamicStateContainer(DynamicLevelState state) {
      Current = state;
      Previous = state;
    }

    public void PushNextState(DynamicLevelState state) {
      Previous = Current;
      Current = state;
      OnCurrentStateChanged?.Invoke();
    }

    public void SetPreviousAndCurrentStates(DynamicLevelState previous, DynamicLevelState current) {
      Previous = previous;
      Current = current;
      OnCurrentStateChanged?.Invoke();
    }
  }
}