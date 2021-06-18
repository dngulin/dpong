using System;

namespace DPong.Meta.Screens.NetworkMenu {
  public readonly struct NetworkMenuEvent {
    public readonly NetworkMenuEventType Type;
    private readonly int _inputSourceIndex;
    private readonly string _strValue;

    private NetworkMenuEvent(NetworkMenuEventType type, int inputSrcIndex, string strValue) {
      Type = type;
      _inputSourceIndex = inputSrcIndex;
      _strValue = strValue;
    }

    public static NetworkMenuEvent Back() {
      return new NetworkMenuEvent(NetworkMenuEventType.Back, default, default);
    }

    public static NetworkMenuEvent Play() {
      return new NetworkMenuEvent(NetworkMenuEventType.Play, default, default);
    }

    public static NetworkMenuEvent NickChanged(string nick) {
      return new NetworkMenuEvent(NetworkMenuEventType.NickChanged, default, nick);
    }

    public static NetworkMenuEvent InputSrcChanged(int index) {
      return new NetworkMenuEvent(NetworkMenuEventType.InputSrcChanged, index, default);
    }

    public static NetworkMenuEvent ServerAddressChanged(string address) {
      return new NetworkMenuEvent(NetworkMenuEventType.ServerAddressChanged, default, address);
    }


    public string GetNickName() {
      if (Type == NetworkMenuEventType.NickChanged)
        return _strValue;
      throw new InvalidOperationException();
    }

    public int GetInputSourceIndex() {
      if (Type == NetworkMenuEventType.InputSrcChanged)
        return _inputSourceIndex;
      throw new InvalidOperationException();
    }

    public string GetServerAddress() {
      if (Type == NetworkMenuEventType.ServerAddressChanged)
        return _strValue;
      throw new InvalidOperationException();
    }
  }
}