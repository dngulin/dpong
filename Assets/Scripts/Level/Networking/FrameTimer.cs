using System.Diagnostics;

namespace DPong.Level.Networking {
  public class FrameTimer {
    private readonly long _tickDuration;
    private readonly Stopwatch _timer;
    private uint _offset;

    public FrameTimer(long tickDuration) {
      _tickDuration = tickDuration;
      _timer = Stopwatch.StartNew();
      _offset = 0;
    }

    public void SyncCurrentFrame(uint currentFrame) {
      _offset = currentFrame;
      _timer.Restart();
    }

    public uint CurrentFrame => _offset + (uint) (_timer.ElapsedMilliseconds / _tickDuration);
  }
}