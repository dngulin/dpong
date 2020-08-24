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
      var (_, max) = GetReachableFramesRange(currentFrame);
      var prevMax = this[max - 1];
      this[max] = new NetworkInputs {
        Left = prevMax.Left,
        Right = prevMax.Right,
        Approved = false,
        MisPredicted = false
      };
    }

    public (uint, uint) GetReachableFramesRange(uint currentFrame) {
      var len = (uint) _inputs.Length;
      var range = len / 2;
      return currentFrame < range ? (0, len - 1) : (currentFrame - range, currentFrame + range);
    }

    public uint CountApproved() {
      var approved = 0u;

      foreach (var input in _inputs)
        if (input.Approved) approved++;

      return approved;
    }

    public uint GetFirstMisPredictedFrame(uint currentFrame) {
      var (min, _) = GetReachableFramesRange(currentFrame);
      for (var frame = min; frame < currentFrame; frame++) {
        if (this[frame].MisPredicted)
          return frame;
      }
      return currentFrame;
    }
  }
}