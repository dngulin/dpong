using System.Diagnostics;
using FxNet.Math;
using UnityEngine;

namespace DPong.Level.Networking {
  public class FrameTimer {
    private readonly FxNum _tickDuration;
    private readonly Stopwatch _timer;
    private uint _offset;

    public FrameTimer(in FxNum tickDuration) {
      _tickDuration = tickDuration;
      _timer = Stopwatch.StartNew();
      _offset = 0;
    }

    public void SyncCurrentFrame(uint currentFrame) {
      _offset = currentFrame;
      _timer.Restart();
    }

    public uint Current => _offset + (uint) (FxNum.FromMillis(_timer.ElapsedMilliseconds) / _tickDuration);

    public float GetBlendingFactor(uint targetFrame) {
      var targetTime = targetFrame * _tickDuration;
      var currentTime = FxNum.FromMillis(_timer.ElapsedMilliseconds) + _offset * _tickDuration;

      var factor = (currentTime - targetTime) / _tickDuration;
      return Mathf.Clamp01((float) factor);
    }
  }
}