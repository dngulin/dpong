using DPong.Level.State;

namespace DPong.Level {
  public interface INetworkInputSource {
    void SendCurrent(int frame);

    Keys GetLeft(int frame);
    Keys GetRight(int frame);

    int GetLastFrameWithChangedInput();
  }
}