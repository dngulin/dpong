using System;
using DPong.Level.State;

namespace DPong.Level.View {
  public interface IDynamicStateContainer {
    event Action OnCurrentStateChanged;
    LevelState Current { get; }
    LevelState Previous { get; }
  }
}