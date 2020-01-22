using System.Collections.Generic;
using DPong.Core.UI;
using DPong.InputSource;
using DPong.Level.Debugging.UI;
using NGIS.Message.Client;
using NGIS.Message.Server;
using NGIS.Session.Client;
using UnityEngine;

namespace DPong.Level.Debugging {
  public class NetworkLevelDebugLoader : MonoBehaviour, IClientSessionWorker {
    private const string GameName = "dPONG";
    private const byte PlayerCount = 2;
    private const int Port = 5081;
    private const ushort Version = 0;

    [SerializeField] private Canvas _canvas;

    [SerializeField] private DbgConnectMenu _menuPrefab;
    [SerializeField] private DbgPopup _popupPrefab;

    private ClientSession _session;
    private NetworkLevelController _level;

    private UISystem _ui;
    private DbgConnectMenu _menu;
    private DbgPopup _statusPopup;

    private void Awake() {
      _ui = new UISystem(_canvas);
      _menu = _ui.InstantiateInLayer(_menuPrefab, UILayer.Background);
      _menu.SetClickListener(ConnectClicked);
    }

    private void FinishClicked() {
      _session?.Dispose();
      _level?.Dispose();

      _menu.Visible = true;
    }

    private void ConnectClicked() {
      _session?.Dispose();

      var cfg = new ClientConfig(GameName, Version, PlayerCount, _menu.HostName, Port, _menu.PlayerName);
      _session = new ClientSession(cfg, this, new NgisUnityLogger());

      _statusPopup = ShowPopup("Connecting to server...");
    }

    private DbgPopup ShowPopup(string text) {
      var popup = _ui.InstantiateWindow(WindowType.Dialog, _popupPrefab, false);
      popup.Init(text, () => {
        popup.Hide();
        FinishClicked();
      });
      popup.OnHideFinish += popup.Destroy;
      popup.Show();
      return popup;
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

    public void ConnectionFailed() {
      if (_statusPopup != null)
        _statusPopup.Hide();

      ShowPopup("Connection failed!");
    }

    public void JoiningToSession() => _statusPopup.UpdateMessage("Joining to session...");
    public void JoinedToSession() => _statusPopup.UpdateMessage("Waiting for players...");

    public void SessionStarted(ServerMsgStart msgStart) {
      _menu.Visible = false;
      _statusPopup.Hide();
      _level = new NetworkLevelController(CreateInputSource(), msgStart);
    }

    public void InputReceived(ServerMsgInput msgInput) => _level.InputReceived(msgInput);
    public (Queue<ClientMsgInputs>, ClientMsgFinished?) Process() => _level.Process();

    public void SessionFinished(ServerMsgFinish msgFinish) {
      var frames = string.Join(", ", msgFinish.Frames);
      var hashes = string.Join(", ", msgFinish.Hashes);
      var (frame, simulations) = _level.SimulationStats;

      ShowPopup($"Finished at [{frames}] with state [{hashes}]\nSimulations: {frame} / {simulations}");
    }

    public void SessionClosedWithError(ClientSessionError errorId, ServerErrorId? serverErrorId = null) {
      if (_statusPopup != null)
        _statusPopup.Hide();

      ShowPopup(serverErrorId.HasValue ? $"{errorId}: {serverErrorId.Value}" : errorId.ToString());
    }
  }
}