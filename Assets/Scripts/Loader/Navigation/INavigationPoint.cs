namespace DPong.Loader.Navigation {
  public interface INavigationPoint {
    void Enter();
    void Suspend();
    void Resume();
    void Exit();
  }
}