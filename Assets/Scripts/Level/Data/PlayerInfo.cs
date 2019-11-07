namespace DPong.Level.Data {
  public class PlayerInfo {
    public readonly string Name;
    public readonly bool IsBot;

    public PlayerInfo(string name, bool isBot) {
      Name = name;
      IsBot = isBot;
    }
  }
}