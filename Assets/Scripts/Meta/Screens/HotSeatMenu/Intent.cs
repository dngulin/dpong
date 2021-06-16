using System;
using DPong.Level.Data;

namespace DPong.Meta.Screens.HotSeatMenu {
  public readonly struct Intent {
    public readonly IntentType Type;
    private readonly Side _side;
    private readonly int _inputSourceIndex;
    private readonly string _nick;

    private Intent(IntentType type, Side side, string nick, int inputSourceIndex) {
      Type = type;
      _side = side;
      _inputSourceIndex = inputSourceIndex;
      _nick = nick;
    }

    public static Intent Back() => new Intent(IntentType.Back, default, default, default);
    public static Intent Play() => new Intent(IntentType.Play, default, default, default);
    public static Intent NickChanged(Side side, string nick) => new Intent(IntentType.NickChanged, side, nick, default);
    public static Intent InputSrcChanged(Side side, int index) => new Intent(IntentType.InputSrcChanged, side, default, index);


    public (Side, string) GetNickChangedData() {
      if (Type == IntentType.NickChanged)
        return (_side, _nick);
      throw new InvalidOperationException();
    }

    public (Side, int) GetInputSourceChangedData() {
      if (Type == IntentType.InputSrcChanged)
        return (_side, _inputSourceIndex);
      throw new InvalidOperationException();
    }
  }
}