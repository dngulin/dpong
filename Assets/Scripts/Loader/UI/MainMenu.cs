using UnityEngine;
using UnityEngine.UI;

namespace DPong.Loader.UI {
  public interface IMainMenuListener {
    void OnHotSeatClicked();
    void OnNetworkClicked();
    void OnExitGameClicked();
  }

  public class MainMenu : Menu {
    [SerializeField] private Button _hotSeatButton;
    [SerializeField] private Button _networkButton;
    [SerializeField] private Button _exitGameButton;

    public void Init(IMainMenuListener listener) {
      _hotSeatButton.onClick.AddListener(listener.OnHotSeatClicked);
      _networkButton.onClick.AddListener(listener.OnNetworkClicked);
      _exitGameButton.onClick.AddListener(listener.OnExitGameClicked);
    }
  }
}