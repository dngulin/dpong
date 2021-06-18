using System;
using DPong.Level.Data;

namespace DPong.Meta.Screens.HotSeatMenu {
  public readonly struct HotSeatMenuEvent {
    public readonly HotSeatMenuEventType Type;
    private readonly Side _side;
    private readonly int _inputSourceIndex;
    private readonly string _nick;

    private HotSeatMenuEvent(HotSeatMenuEventType type, Side side, string nick, int inputSourceIndex) {
      Type = type;
      _side = side;
      _inputSourceIndex = inputSourceIndex;
      _nick = nick;
    }

    public static HotSeatMenuEvent Back() {
      return new HotSeatMenuEvent(HotSeatMenuEventType.Back, default, default, default);
    }

    public static HotSeatMenuEvent Play() {
      return new HotSeatMenuEvent(HotSeatMenuEventType.Play, default, default, default);
    }

    public static HotSeatMenuEvent NickChanged(Side side, string nick) {
      return new HotSeatMenuEvent(HotSeatMenuEventType.NickChanged, side, nick, default);
    }

    public static HotSeatMenuEvent InputSrcChanged(Side side, int index) {
      return new HotSeatMenuEvent(HotSeatMenuEventType.InputSrcChanged, side, default, index);
    }


    public (Side, string) GetNickChangedData() {
      if (Type == HotSeatMenuEventType.NickChanged)
        return (_side, _nick);
      throw new InvalidOperationException();
    }

    public (Side, int) GetInputSourceChangedData() {
      if (Type == HotSeatMenuEventType.InputSrcChanged)
        return (_side, _inputSourceIndex);
      throw new InvalidOperationException();
    }
  }
}