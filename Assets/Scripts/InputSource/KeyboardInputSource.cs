using DPong.Level;
using DPong.Level.State;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace DPong.InputSource {
  public class KeyboardInputSource : IInputSource {
    private readonly ButtonControl _up;
    private readonly ButtonControl _down;

    public KeyboardInputSource(Keyboard keyboard, Key up, Key down) {
      _up = keyboard[up];
      _down = keyboard[down];
    }

    public Keys GetKeys() {
      var keys = Keys.None;

      if (_up.isPressed) keys |= Keys.Up;
      if (_down.isPressed) keys |= Keys.Down;

      return keys;
    }
  }
}