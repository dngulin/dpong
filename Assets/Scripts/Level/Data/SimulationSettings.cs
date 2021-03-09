using FxNet.Math;

namespace DPong.Level.Data {
  public class SimulationSettings {
    public readonly FxNum TickDuration;
    public readonly ulong Seed;

    public SimulationSettings(in FxNum tickDuration, in ulong seed) {
      TickDuration = tickDuration;
      Seed = seed;
    }
  }
}