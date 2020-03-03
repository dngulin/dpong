namespace DPong.InputSource.Data {
  public readonly struct InputSourceConfig {
    public readonly InputSourceType Type;
    public readonly int DeviceId;
    public readonly int LayoutIndex;

    public InputSourceConfig(InputSourceType type, int deviceId, int layoutIndex) {
      Type = type;
      DeviceId = deviceId;
      LayoutIndex = layoutIndex;
    }
  }
}