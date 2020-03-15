using System;
using DPong.Game.Navigation;
using DPong.InputSource;
using DPong.Save;
using DPong.UI;

namespace DPong.Game.Screens.NetworkGame {
  public class NetworkGameScreen : INavigationPoint, ITickable, IDisposable {
    public NetworkGameScreen(SaveSystem save, InputSourceProvider inputSources, UISystem ui, Navigator navigator) { }

    public void Dispose() { }

    void INavigationPoint.Enter() {
      throw new System.NotImplementedException();
    }

    void INavigationPoint.Suspend() {
      throw new System.NotImplementedException();
    }

    void INavigationPoint.Resume() {
      throw new System.NotImplementedException();
    }

    void INavigationPoint.Exit() {
      throw new System.NotImplementedException();
    }

    void ITickable.FixedTick() {
    }

    void ITickable.DynamicTick(float dt) {
    }
  }
}