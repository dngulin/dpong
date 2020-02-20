using DPong.Game.Navigation;

namespace DPong.Game.Screens {
  public readonly struct MainScreenTransitions {
    public readonly NavigationToken HotSeatMenu;
    public readonly NavigationToken NetworkMenu;

    public MainScreenTransitions(NavigationToken hotSeatMenu, NavigationToken networkMenu) {
      HotSeatMenu = hotSeatMenu;
      NetworkMenu = networkMenu;
    }
  }
}