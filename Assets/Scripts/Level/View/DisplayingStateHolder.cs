using System;
using DPong.Level.State;

namespace DPong.Level.View {
  public class DisplayingStateHolder : IDisplayingStateHolder {
    public event Action OnCurrentStateChanged;

    public LevelState Current { private set; get; }
    public LevelState Previous { private set; get; }

    public DisplayingStateHolder(LevelState state) {
      Current = state;
      Previous = state;
    }

    public void PushNextState(LevelState state) {
      Previous = Current;
      Current = state;
      OnCurrentStateChanged?.Invoke();
    }

    public void SetPreviousAndCurrentStates(LevelState previous, LevelState current) {
      Previous = previous;
      Current = current;
      OnCurrentStateChanged?.Invoke();
    }
  }
}