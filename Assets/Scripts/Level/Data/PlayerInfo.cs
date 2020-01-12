namespace DPong.Level.Data {
  public class PlayerInfo {
    public readonly string Name;
    public readonly PlayerType Type;

    public PlayerInfo(string name, PlayerType type) {
      Name = name;
      Type = type;
    }
  }
}