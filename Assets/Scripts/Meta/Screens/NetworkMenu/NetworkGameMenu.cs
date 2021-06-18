using System.Collections.Generic;
using DPong.Meta.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Meta.Screens.NetworkMenu {
  public class NetworkGameMenu : SimpleMenu {
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _backButton;

    [SerializeField] private InputField _playerName;
    [SerializeField] private Dropdown _inputSources;

    [SerializeField] private InputField _serverAddress;

    public readonly Queue<NetworkMenuEvent> Events = new Queue<NetworkMenuEvent>();

    private void Awake() {
      _playButton.onClick.AddListener(() => Events.Enqueue(NetworkMenuEvent.Play()));
      _backButton.onClick.AddListener(() => Events.Enqueue(NetworkMenuEvent.Back()));

      _playerName.onEndEdit.AddListener(nick => Events.Enqueue(NetworkMenuEvent.NickChanged(nick)));
      _inputSources.onValueChanged.AddListener(index => Events.Enqueue(NetworkMenuEvent.InputSrcChanged(index)));

      _serverAddress.onEndEdit.AddListener(address => Events.Enqueue(NetworkMenuEvent.ServerAddressChanged(address)));
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