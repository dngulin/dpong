using UnityEngine.InputSystem;

namespace DPong.InputSource {
  public class KeyboardInputSource : AbstractButtonInputSource {
    public KeyboardInputSource(Keyboard keyboard, Key up, Key down) : base(keyboard[up], keyboard[down]) {
    }
  }
}