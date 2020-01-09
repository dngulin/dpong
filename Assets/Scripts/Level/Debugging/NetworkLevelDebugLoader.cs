using System;
using System.Collections.Generic;
using DPong.InputSource;
using NGIS.Message.Client;
using NGIS.Message.Server;
using NGIS.Session.Client;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Level.Debugging {
  public class NetworkLevelDebugLoader : MonoBehaviour, IClientSessionWorker {
    private const string GameName = "dPONG";
    private const byte PlayerCount = 2;
    private const int Port = 5081;
    private const ushort Version = 0;

    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _startButton;
    [SerializeField] private InputField _host;
    [SerializeField] private InputField _name;
    [SerializeField] private Text _status;

    private ClientSession _session;
    private NetworkLevelController _level;

    private void Awake() {
      _startButton.onClick.AddListener(ConnectClicked);
    }

    private void ConnectClicked() {
      var cfg = new ClientConfig(GameName, Version, PlayerCount, _host.text, Port, _name.text);

      _session?.Dispose();
      try {
        _panel.SetActive(false);
        _status.text = "Connecting...";
        _session = new ClientSession(cfg, this, new NgisUnityLogger());
      }
      catch (Exception e) {
        _panel.SetActive(true);
        _status.text = e.Message;
      }
    }

    private void Update() {
      _session?.Process();
    }

    private void OnDestroy() {
      _session?.Dispose();
      _level?.Dispose();
    }

    private static ILocalInputSource CreateInputSource() {
      var left = new KeyBindings { Up = KeyCode.W, Down = KeyCode.S };
      var right = new KeyBindings { Up = KeyCode.P, Down = KeyCode.L };
      return new KeyboardInputSource(left, right);
    }

    public void SessionStarted(ServerMsgStart msgStart) {
      _level = new NetworkLevelController(CreateInputSource(), msgStart);
      _status.text = "Session started";
    }

    public void InputReceived(ServerMsgInput msgInput) => _level.InputReceived(msgInput);
    public (Queue<ClientMsgInputs>, ClientMsgFinished?) Process() => _level.Process();

    public void SessionFinished(ServerMsgFinish msgFinish) {
      // TODO: Show level exit UI
      var frames = string.Join(", ", msgFinish.Frames);
      var hashes = string.Join(", ", msgFinish.Hashes);
      _status.text = $"Finished at [{frames}] with state [{hashes}]";
    }

    public void SessionClosedWithError(ClientSessionError errorId, ServerErrorId? serverErrorId = null) {
      _status.text = $"{errorId}:{serverErrorId}";

      if (_level != null) {
        // TODO: Show level exit UI
      }
      else {
        _panel.SetActive(true);
      }
    }
  }
}