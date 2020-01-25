using DPong.Level;
using DPong.Level.State;
using UnityEngine.InputSystem;

namespace DPong.InputSource {
  public class KeyboardInputSource : ILocalInputSource {
    private readonly KeyBindings _leftBindings;
    private readonly KeyBindings _rightBindings;

    public KeyboardInputSource(KeyBindings leftBindings, KeyBindings rightBindings) {
      _leftBindings = leftBindings;
      _rightBindings = rightBindings;
    }

    public Keys GetLeft() => GetKeys(_leftBindings);

    public Keys GetRight() => GetKeys(_rightBindings);

    private static Keys GetKeys(KeyBindings bindings) {
      var keys = Keys.None;

      var keyboard = Keyboard.current;

      if (keyboard[bindings.Up].isPressed) keys |= Keys.Up;
      if (keyboard[bindings.Down].isPressed) keys |= Keys.Down;

      return keys;
    }
  }
}