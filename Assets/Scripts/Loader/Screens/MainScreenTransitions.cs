using DPong.Loader.Navigation;

namespace DPong.Loader.Screens {
  public readonly struct MainScreenTransitions {
    public readonly NavigationToken HotSeatMenu;
    public readonly NavigationToken NetworkMenu;

    public MainScreenTransitions(NavigationToken hotSeatMenu, NavigationToken networkMenu) {
      HotSeatMenu = hotSeatMenu;
      NetworkMenu = networkMenu;
    }
  }
}