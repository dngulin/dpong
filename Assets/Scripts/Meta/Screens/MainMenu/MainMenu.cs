using DPong.Meta.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Meta.Screens.MainMenu {
  public class MainMenu : SimpleMenu {
    [SerializeField] private Button _hotSeatButton;
    [SerializeField] private Button _networkButton;
    [SerializeField] private Button _exitGameButton;

    private MainMenuEvent _pendingEvent;

    public MainMenuEvent GetEvent() {
      var evt = _pendingEvent;
      _pendingEvent = MainMenuEvent.None;
      return evt;
    }

    private void Awake() {
      _hotSeatButton.onClick.AddListener(() => _pendingEvent = MainMenuEvent.HotSeatMenu);
      _networkButton.onClick.AddListener(() => _pendingEvent = MainMenuEvent.NetworkMenu);
      _exitGameButton.onClick.AddListener(() => _pendingEvent = MainMenuEvent.Exit);
    }
  }
}