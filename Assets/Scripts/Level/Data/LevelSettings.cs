namespace DPong.Level.Data {
  public class LevelSettings {
    public readonly PlayerInfo PlayerLeft;
    public readonly PlayerInfo PlayerRight;

    public readonly SimulationSettings Simulation;

    public readonly int HitPoints = 5;
    public readonly PaceSettings Pace = new PaceSettings();
    public readonly BoardSettings Board = new BoardSettings();
    public readonly BallSettings Ball = new BallSettings();
    public readonly BlockerSettings Blocker = new BlockerSettings();

    public LevelSettings(PlayerInfo playerLeft, PlayerInfo playerRight, SimulationSettings simulationSettings) {
      PlayerLeft = playerLeft;
      PlayerRight = playerRight;
      Simulation = simulationSettings;
    }
  }
}