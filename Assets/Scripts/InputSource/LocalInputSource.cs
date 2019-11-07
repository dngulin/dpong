using DPong.Level;
using DPong.Level.State;
using UnityEngine;

namespace DPong.InputSource {
  public class LocalInputSource : ILocalInputSource {
    private readonly KeyBindings _leftBindings;
    private readonly KeyBindings _rightBindings;

    public LocalInputSource(KeyBindings leftBindings, KeyBindings rightBindings) {
      _leftBindings = leftBindings;
      _rightBindings = rightBindings;
    }

    public Keys GetLeft() => GetKeys(_leftBindings);

    public Keys GetRight() => GetKeys(_rightBindings);

    private static Keys GetKeys(KeyBindings bindings) {
      var keys = Keys.None;

      if (Input.GetKey(bindings.Up)) keys |= Keys.Up;
      if (Input.GetKey(bindings.Down)) keys |= Keys.Down;

      return keys;
    }
  }
}