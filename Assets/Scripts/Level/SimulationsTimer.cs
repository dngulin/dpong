namespace DPong.Level {
  public class SimulationsTimer {
    private readonly float _frameDuration;
    private float _time;

    public SimulationsTimer(float frameDuration) => _frameDuration = frameDuration;

    public (int Simulations, float BlendingFactor) Tick(float dt) {
      _time += dt;
      var simulations = (int)(_time / _frameDuration);

      _time %= _frameDuration;
      var blendingFactor = _time / _frameDuration;

      return (simulations, blendingFactor);
    }
  }
}