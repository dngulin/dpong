using System.Diagnostics;
using UnityEngine;

namespace DPong.Level.Networking {
  public class FrameTimer {
    private readonly long _tickDurationMs;
    private readonly Stopwatch _timer;
    private uint _offset;

    public FrameTimer(long tickDurationMs) {
      _tickDurationMs = tickDurationMs;
      _timer = Stopwatch.StartNew();
      _offset = 0;
    }

    public void SyncCurrentFrame(uint currentFrame) {
      _offset = currentFrame;
      _timer.Restart();
    }

    public uint Current => _offset + (uint) (_timer.ElapsedMilliseconds / _tickDurationMs);

    public float GetBlendingFactor(uint targetFrame) {
      var targetMs = targetFrame * _tickDurationMs;
      var currentMs = _timer.ElapsedMilliseconds + _offset * _tickDurationMs;

      var factor = (float)(currentMs - targetMs) / _tickDurationMs;
      return Mathf.Clamp01(factor);
    }
  }
}