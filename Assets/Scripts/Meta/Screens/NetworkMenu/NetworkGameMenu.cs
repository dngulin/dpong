using System.Collections.Generic;
using DPong.Meta.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Meta.Screens.NetworkMenu {
  public interface INetworkGameMenuListener {
    void PlayClicked();
    void BackClicked();
    void PlayerNameChanged(string name);
    void InputSourceChanged(int srcIndex);
    void ServerAddressChanged(string address);
  }

  public class NetworkGameMenu : SimpleMenu {
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _backButton;

    [SerializeField] private InputField _playerName;
    [SerializeField] private Dropdown _inputSources;

    [SerializeField] private InputField _serverAddress;

    public void Init(INetworkGameMenuListener listener) {
      _playButton.onClick.AddListener(listener.PlayClicked);
      _backButton.onClick.AddListener(listener.BackClicked);

      _playerName.onEndEdit.AddListener(listener.PlayerNameChanged);
      _inputSources.onValueChanged.AddListener(listener.InputSourceChanged);

      _serverAddress.onEndEdit.AddListener(listener.ServerAddressChanged);
    }

    public void SetPlayerName(string playerName) => _playerName.text = playerName;

    public void SetServerAddress(string serverAddress) => _serverAddress.text = serverAddress;

    public void SetInputSources(IReadOnlyList<string> sources, int selected) {
      _inputSources.options.Clear();

      foreach (var source in sources)
        _inputSources.options.Add(new Dropdown.OptionData(source));

      _inputSources.value = selected;
    }
  }
}