using System;
using DPong.Meta.Screens.HotSeatMenu;
using DPong.Meta.Screens.NetworkMenu;
using DPong.StateTracker;
using DPong.UI;
using Object = UnityEngine.Object;

namespace DPong.Meta.Screens.MainMenu {
  public class MainMenuScreen : GameState<DPong> {
    private MainMenu _menu;

    public override void Start(DPong game) {
      var prefab = game.Assets.LoadFromPrefab<MainMenu>("Assets/Content/Meta/Prefabs/MainMenu.prefab");
      _menu = game.Ui.Instantiate(prefab, UILayer.Background, true);
    }

    public override Transition Tick(DPong game, float dt) {
      var evt = _menu.GetEvent();

      switch (evt) {
        case MainMenuEvent.None: return Transition.None();
        case MainMenuEvent.HotSeatMenu: return Transition.Push(new HotSeatMenuScreen(game.Save));
        case MainMenuEvent.NetworkMenu: return Transition.Push(new NetworkMenuScreen(game.Save));
        case MainMenuEvent.Exit: return Transition.Pop();
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public override void Pause(DPong game) => _menu.Hide();
    public override void Resume(DPong game) => _menu.Show();
    public override void Finish(DPong game) => Object.Destroy(_menu.gameObject);
  }
}