using System;
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
    private DbgPopup _waitingPopup;

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
      var cfg = new ClientConfig(GameName, Version, PlayerCount, _menu.HostName, Port, _menu.PlayerName);

      _session?.Dispose();
      var connectingPopup = ShowPopup("Connecting...");
      try {
        _session = new ClientSession(cfg, this, new NgisUnityLogger());
      }
      catch (Exception e) {
        ShowPopup(e.Message);
      }

      connectingPopup.Holder.Hide();
    }

    private DbgPopup ShowPopup(string text) {
      var popup = _ui.InstantiateWindow(WindowType.Dialog, _popupPrefab, false);
      popup.Init(text, () => {
        popup.Holder.Hide();
        FinishClicked();
      });
      popup.Holder.OnHideFinish += () => Destroy(popup.Holder.gameObject);
      popup.Holder.Show();
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

    public void JoinedToSession() {
      _waitingPopup = ShowPopup("Waiting for players...");
    }

    public void SessionStarted(ServerMsgStart msgStart) {
      _menu.Visible = false;
      _waitingPopup.Holder.Hide();
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
      if (_waitingPopup != null)
        _waitingPopup.Holder.Hide();

      ShowPopup(serverErrorId.HasValue ? $"{errorId}: {serverErrorId.Value}" : errorId.ToString());
    }
  }
}