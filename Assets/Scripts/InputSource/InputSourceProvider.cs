using System;
using System.Collections.Generic;
using DPong.InputSource.Data;
using DPong.InputSource.Sources;
using DPong.Level;
using DPong.Localization;
using UnityEngine.InputSystem;

namespace DPong.InputSource {
  public class InputSourceProvider {
    private const int KeyboardLayoutVariants = 2;

    private readonly List<InputSourceDescriptor> _descriptors = new List<InputSourceDescriptor>();
    private readonly List<string> _names = new List<string>();

    public IReadOnlyList<InputSourceDescriptor> Descriptors => _descriptors;
    public IReadOnlyList<string> Names => _names;

    public InputSourceProvider() {
      Resfresh();
    }

    public void Resfresh() {
      _descriptors.Clear();
      _names.Clear();

      if (Keyboard.current != null) {
        var id = Keyboard.current.deviceId;
        for (var layoutVariant = 0; layoutVariant < KeyboardLayoutVariants; layoutVariant++) {
          _descriptors.Add(new InputSourceDescriptor(InputSourceType.Keyboard, id, layoutVariant));
          _names.Add(GetKeyboardSourceName(layoutVariant));
        }
      }

      var gamepads = Gamepad.all;
      for (var gpIdx = 0; gpIdx < gamepads.Count; gpIdx++) {
        var gamepad = gamepads[gpIdx];
        _descriptors.Add(new InputSourceDescriptor(InputSourceType.Gamepad, gamepad.deviceId, 0));
        _names.Add(GetGamepadSourceName(gamepad, gpIdx));
      }
    }

    private static string GetKeyboardSourceName(int layoutId) {
      var baseName = Tr._("Keyboard");
      string layout;

      switch (layoutId) {
        case 0:
          layout = "W/S";
          break;
        case 1:
          layout = Tr._("Arrows");
          break;

        default: throw new IndexOutOfRangeException();
      }

      return $"{baseName} {layout}";
    }

    private static string GetGamepadSourceName(Gamepad gamepad, int index) {
      var baseName = Tr._("Gamepad");
      var deviceName = gamepad.name ?? Tr._p("device name", "Unknown");
      return $"{baseName} '{deviceName}' ({index + 1})";
    }

    public IInputSource CreateSource(in InputSourceDescriptor descriptor) {
      switch (descriptor.Type) {
        case InputSourceType.Keyboard: return CreateKeyboardSource(descriptor.DeviceId, descriptor.LayoutVariant);
        case InputSourceType.Gamepad: return CreateGamepadSource(descriptor.DeviceId);
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private IInputSource CreateGamepadSource(int deviceId) {
      foreach (var gamepad in Gamepad.all) {
        if (gamepad.deviceId == deviceId)
          return new GamePadInputSource(gamepad);
      }

      return null;
    }

    private static IInputSource CreateKeyboardSource(int deviceId, int layout) {
      var kbd = Keyboard.current;
      if (kbd == null || kbd.deviceId != deviceId || layout < 0 || layout >= KeyboardLayoutVariants)
        return null;

      var (keyUp, keyDown) = layout == 0 ? (Key.W, Key.S) : (Key.UpArrow, Key.DownArrow);

      return new KeyboardInputSource(kbd, keyUp, keyDown);
    }
  }
}