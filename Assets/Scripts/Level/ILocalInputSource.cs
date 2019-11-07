using DPong.Level.State;

namespace DPong.Level {
  public interface ILocalInputSource {
    Keys GetLeft();
    Keys GetRight();
  }
}