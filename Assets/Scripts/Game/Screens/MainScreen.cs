using DPong.Game.Navigation;
using DPong.Game.UI;
using DPong.UI;

namespace DPong.Game.Screens {
  public class MainScreen : INavigationPoint, IMainMenuListener {
    private readonly Navigator _navigator;
    private readonly MainScreenTransitions _transitions;

    private readonly MainMenu _menu;

    public MainScreen(UISystem ui, MenuPrefabs prefabs, Navigator navigator, MainScreenTransitions transitions) {
      _navigator = navigator;
      _transitions = transitions;

      _menu = ui.Instantiate(prefabs.MainMenu, UILayer.Background, false);
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