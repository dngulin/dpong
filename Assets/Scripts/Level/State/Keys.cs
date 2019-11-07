using System;

namespace DPong.Level.State {
  [Flags]
  public enum Keys: byte {
    None = 0,
    Up = 1,
    Down = 1 << 1
  }

  public static class KeysExtensions {
    public static bool HasKey(this Keys value, Keys key) {
      return (value & key) != 0;
    }
  }
}