namespace DPong.Game {
  public interface ITickable {
    void FixedTick();
    void DynamicTick(float dt);
  }
}