using DPong.Level.State;

namespace DPong.Level.Networking {
  public class StateBuffer {
    private readonly LevelState[] _states;

    public StateBuffer(int length) {
      _states = new LevelState[length];
    }

    public uint Count => (uint) _states.Length;

    public ref LevelState this[uint index] => ref _states[index % _states.Length];
  }
}