using PGM.Random;

namespace DPong.Level.Data {
  public class SimulationSettings {
    public readonly long FrameTime;
    public readonly PcgState? RandomState;

    public SimulationSettings(long frameTime, PcgState? randomState) {
      FrameTime = frameTime;
      RandomState = randomState;
    }
  }
}