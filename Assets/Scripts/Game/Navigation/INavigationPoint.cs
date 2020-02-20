namespace DPong.Game.Navigation {
  public interface INavigationPoint {
    void Enter();
    void Suspend();
    void Resume();
    void Exit();
  }
}