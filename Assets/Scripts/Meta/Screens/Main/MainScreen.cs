using DPong.Assets;
using DPong.Meta.Navigation;
using DPong.UI;
using UnityEngine;

namespace DPong.Meta.Screens.Main {
  public class MainScreen : INavigationPoint, IMainMenuListener {
    public readonly struct Transitions {
      public readonly NavigationToken HotSeatMenu;
      public readonly NavigationToken NetworkMenu;

      public Transitions(NavigationToken hotSeatMenu, NavigationToken networkMenu) {
        HotSeatMenu = hotSeatMenu;
        NetworkMenu = networkMenu;
      }
    }

    private readonly AssetLoader _assetLoader;
    private readonly UISystem _ui;
    private readonly Navigator _navigator;
    private readonly Transitions _transitions;

    private MainMenu _menu;

    public MainScreen(AssetLoader assetLoader, UISystem ui, Navigator navigator, Transitions transitions) {
      _assetLoader = assetLoader;
      _ui = ui;
      _navigator = navigator;
      _transitions = transitions;
    }

    void INavigationPoint.Enter() {
      var prefab = _assetLoader.LoadFromPrefab<MainMenu>("Assets/Content/Meta/Prefabs/MainMenu.prefab");
      _menu = _ui.Instantiate(prefab, UILayer.Background, true);
      _menu.Init(this);
    }

    void INavigationPoint.Suspend() => _menu.Hide();
    void INavigationPoint.Resume() => _menu.Show();

    void INavigationPoint.Exit() {
      if (_menu != null) {
        Object.Destroy(_menu.gameObject);
        _menu = null;
      }

      ExitGame();
    }

    void INavigationPoint.Tick(float dt) {}

    void IMainMenuListener.OnHotSeatClicked() => _navigator.Enter(_transitions.HotSeatMenu);
    void IMainMenuListener.OnNetworkClicked() => _navigator.Enter(_transitions.NetworkMenu);
    void IMainMenuListener.OnExitGameClicked() => _navigator.Exit(this);

    private static void ExitGame() {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
  }
}