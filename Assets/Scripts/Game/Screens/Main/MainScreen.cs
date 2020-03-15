using DPong.Game.Navigation;
using DPong.UI;
using UnityEngine;

namespace DPong.Game.Screens.Main {
  public class MainScreen : INavigationPoint, IMainMenuListener {
    public readonly struct Transitions {
      public readonly NavigationToken HotSeatMenu;
      public readonly NavigationToken NetworkMenu;

      public Transitions(NavigationToken hotSeatMenu, NavigationToken networkMenu) {
        HotSeatMenu = hotSeatMenu;
        NetworkMenu = networkMenu;
      }
    }

    private readonly Navigator _navigator;
    private readonly Transitions _transitions;

    private readonly MainMenu _menu;

    public MainScreen(UISystem ui, Navigator navigator, Transitions transitions) {
      _navigator = navigator;
      _transitions = transitions;

      _menu = ui.Instantiate(Resources.Load<MainMenu>("MainMenu"), UILayer.Background, false);
      _menu.Init(this);
    }

    void INavigationPoint.Enter() => _menu.Show();
    void INavigationPoint.Suspend() => _menu.Hide();
    void INavigationPoint.Resume() => _menu.Show();
    void INavigationPoint.Exit() => ExitGame();

    void IMainMenuListener.OnHotSeatClicked() => _navigator.Enter(_transitions.HotSeatMenu);
    void IMainMenuListener.OnNetworkClicked() => _navigator.Enter(_transitions.NetworkMenu);
    void IMainMenuListener.OnExitGameClicked() => ExitGame();

    private static void ExitGame() {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
  }
}