using System;
using DPong.Level.State;

namespace DPong.Level.View {
  public interface IDynamicStateContainer {
    event Action OnCurrentStateChanged;
    DynamicLevelState Current { get; }
    DynamicLevelState Previous { get; }
  }
}