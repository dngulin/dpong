namespace DPong.Level.Data {
  public class LevelInfo {
    public readonly PlayerInfo Left;
    public readonly PlayerInfo Right;

    public readonly SimulationSettings Settings;

    public LevelInfo(PlayerInfo left, PlayerInfo right, SimulationSettings settings) {
      Left = left;
      Right = right;
      Settings = settings;
    }
  }
}