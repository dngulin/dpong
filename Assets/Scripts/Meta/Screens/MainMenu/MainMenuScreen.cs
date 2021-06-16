using System;
using DPong.Meta.Screens.HotSeatMenu;
using DPong.Meta.Screens.NetworkMenu;
using DPong.StateTracker;
using DPong.UI;
using Object = UnityEngine.Object;

namespace DPong.Meta.Screens.MainMenu {
  public class MainMenuScreen : GameState<DPong>, IMainMenuListener {
    private enum Intent {
      None,
      HotSeatMenu,
      NetworkMenu,
      Exit
    }

    private MainMenu _menu;
    private Intent _intent;

    void IMainMenuListener.OnHotSeatClicked() => _intent = Intent.HotSeatMenu;
    void IMainMenuListener.OnNetworkClicked() => _intent = Intent.NetworkMenu;
    void IMainMenuListener.OnExitGameClicked() => _intent = Intent.Exit;

    public override void Start(DPong game) {
      var prefab = game.Assets.LoadFromPrefab<MainMenu>("Assets/Content/Meta/Prefabs/MainMenu.prefab");
      _menu = game.Ui.Instantiate(prefab, UILayer.Background, true);
      _menu.Init(this);
    }

    public override void Pause(DPong game) => _menu.Hide();
    public override void Resume(DPong game) => _menu.Show();

    public override void Finish(DPong game) {
      if (_menu != null) {
        Object.Destroy(_menu.gameObject);
        _menu = null;
      }
    }

    public override Transition Tick(DPong game, float dt) {
      var intent = _intent;
      _intent = Intent.None;

      switch (intent) {
        case Intent.None: return Transition.None();
        case Intent.HotSeatMenu: return Transition.Push(new HotSeatMenuScreen(game.Save));
        case Intent.NetworkMenu: return Transition.Push(new NetworkMenuScreen(game.Save));
        case Intent.Exit: return Transition.Pop();
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}