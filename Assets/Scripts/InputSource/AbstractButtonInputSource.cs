using DPong.Level;
using DPong.Level.State;
using UnityEngine.InputSystem.Controls;

namespace DPong.InputSource {
  public abstract class AbstractButtonInputSource : IInputSource {
    private readonly ButtonControl _up;
    private readonly ButtonControl _down;

    protected AbstractButtonInputSource(ButtonControl up, ButtonControl down) {
      _up = up;
      _down = down;
    }

    public Keys GetKeys() {
      var keys = Keys.None;

      if (_up.isPressed) keys |= Keys.Up;
      if (_down.isPressed) keys |= Keys.Down;

      return keys;
    }
  }
}