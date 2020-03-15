namespace DPong.InputSource.Data {
  public readonly struct InputSourceDescriptor {
    public readonly InputSourceType Type;
    public readonly int DeviceId;
    public readonly int LayoutVariant;

    public InputSourceDescriptor(InputSourceType type, int deviceId, int layoutVariant) {
      Type = type;
      DeviceId = deviceId;
      LayoutVariant = layoutVariant;
    }

    public static bool operator ==(InputSourceDescriptor l, InputSourceDescriptor r) => l.Equals(r);
    public static bool operator !=(InputSourceDescriptor l, InputSourceDescriptor r) => !(l == r);

    public bool Equals(InputSourceDescriptor other) {
      return Type == other.Type && DeviceId == other.DeviceId && LayoutVariant == other.LayoutVariant;
    }

    public override bool Equals(object obj) {
      return obj is InputSourceDescriptor other && Equals(other);
    }

    public override int GetHashCode() {
      unchecked {
        var hashCode = (int) Type;
        hashCode = (hashCode * 397) ^ DeviceId;
        hashCode = (hashCode * 397) ^ LayoutVariant;
        return hashCode;
      }
    }
  }
}