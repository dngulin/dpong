using System;
using DPong.Level.State;

namespace DPong.Level.View {
  public interface IDisplayingStateHolder {
    event Action OnCurrentStateChanged;
    LevelState Current { get; }
    LevelState Previous { get; }
  }
}