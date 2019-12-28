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

    public void HandleFrameIncremented(uint frame) {
      var (_, max) = GetWindow(frame);
      var prevMax = this[max - 1];
      this[max] = new NetworkInputs {
        Left = prevMax.Left,
        Right = prevMax.Right,
        Approved = false,
        MisPredicted = false
      };
    }

    public (uint, uint) GetWindow(uint frame) {
      var len = (uint) _inputs.Length;
      var range = len / 2;
      return frame < range ? (0, len - 1) : (frame - range, frame + range);
    }

    public uint CountApproved() {
      var approved = 0u;

      foreach (var input in _inputs)
        if (input.Approved) approved++;

      return approved;
    }
  }
}