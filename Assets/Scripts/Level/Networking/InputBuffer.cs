using System.ComponentModel;

namespace DPong.Level.Networking {
  public class InputBuffer {
    private readonly NetworkInputs[] _inputs;

    public InputBuffer(int length) {
      if (length % 2 != 1)
        throw new InvalidEnumArgumentException("Buffer size should be an odd number!");

      _inputs = new NetworkInputs[length];
    }

    public ref NetworkInputs this[uint index] => ref _inputs[index % _inputs.Length];

    public void HandleFrameIncremented(uint currentFrame) {
      var (_, max) = GetFramesRange(currentFrame);
      var prevMax = this[max - 1];
      this[max] = new NetworkInputs {
        Left = prevMax.Left,
        Right = prevMax.Right,
        Approved = false,
        MisPredicted = false
      };
    }

    public (uint, uint) GetFramesRange(uint currentFrame) {
      var len = (uint) _inputs.Length;
      var range = len / 2;
      return currentFrame < range ? (0, len - 1) : (currentFrame - range, currentFrame + range);
    }

    public uint GetFirstMisPredictedFrame(uint currentFrame) {
      var (min, _) = GetFramesRange(currentFrame);
      for (var frame = min; frame < currentFrame; frame++) {
        if (this[frame].MisPredicted)
          return frame;
      }
      return currentFrame;
    }

    public uint GetMaxReachableFrame(uint currentFrame) {
      var advanceSteps = CountApproved();

      var length = (uint) _inputs.Length;
      var range = length / 2;
      if (currentFrame < range)
        advanceSteps += range - currentFrame;

      return currentFrame + advanceSteps;
    }

    private uint CountApproved() {
      var approved = 0u;

      foreach (var input in _inputs)
        if (input.Approved) approved++;

      return approved;
    }
  }
}