using PGM.Random;

namespace DPong.Level.Data {
  public class SimulationSettings {
    public readonly long TickDuration;
    public readonly PcgState? RandomState;

    public SimulationSettings(long tickDuration, PcgState? randomState) {
      TickDuration = tickDuration;
      RandomState = randomState;
    }
  }
}