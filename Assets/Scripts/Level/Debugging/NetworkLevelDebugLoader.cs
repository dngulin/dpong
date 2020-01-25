using System;
using System.Collections.Generic;
using DPong.InputSource;
using DPong.Level.Debugging.UI;
using DPong.UI;
using NGIS.Message.Client;
using NGIS.Message.Server;
using NGIS.Session.Client;
using UnityEngine;
using UnityEngine.InputSystem;

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
      _menu = _ui.Instantiate(_menuPrefab, UILayer.Background);
      _menu.SetClickListener(ConnectClicked);
    }

    private DbgPopup CreatePopup(string text) {
      var popup = _ui.InstantiateWindow(WindowType.Dialog, _popupPrefab, false);
      popup.Init(text, () => {
        popup.Hide();
        FinishClicked();
      });
      popup.OnHideFinish += popup.Destroy;
      popup.Show();
      return popup;
    }

    private static ILocalInputSource CreateInputSource() {
      var left = new KeyBindings { Up = Key.W, Down = Key.S };
      var right = new KeyBindings { Up = Key.P, Down = Key.L };
      return new KeyboardInputSource(left, right);
    }

    private void FinishClicked() {
      _session?.Dispose();
      _level?.Dispose();

      _menu.Visible = true;
    }

    private void ConnectClicked() {
      _session?.Dispose();

      var cfg = new ClientConfig(GameName, Version, PlayerCount, _menu.HostName, Port, _menu.PlayerName);
      try {
        _session = new ClientSession(cfg, this, new NgisUnityLogger());
      }
      catch (Exception e) {
        CreatePopup(e.Message);
        return;
      }

      _statusPopup = CreatePopup("Connecting...");
    }

    private void Update() {
      _session?.Process();
    }

    private void OnDestroy() {
      _session?.Dispose();
      _level?.Dispose();
    }

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

      CreatePopup($"Finished at [{frames}] with state [{hashes}]\nSimulations: {frame} / {simulations}");
    }

    public void SessionClosedWithError(ClientSessionError errorId, ServerErrorId? serverErrorId = null) {
      if (_statusPopup != null)
        _statusPopup.Hide();

      CreatePopup(serverErrorId.HasValue ? $"{errorId}: {serverErrorId.Value}" : errorId.ToString());
    }
  }
}