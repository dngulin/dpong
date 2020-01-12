using System;
using System.Collections.Generic;
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

    [SerializeField] private ConnectPanel _panel;
    [SerializeField] private Popup _popup;

    private ClientSession _session;
    private NetworkLevelController _level;

    private void Awake() {
      _panel.SetClickListener(ConnectClicked);
      _popup.SetClickListener(FinishClicked);
    }

    private void FinishClicked() {
      _session?.Dispose();
      _level?.Dispose();

      _popup.Visible = false;
      _panel.Visible = true;
    }

    private void ConnectClicked() {
      var cfg = new ClientConfig(GameName, Version, PlayerCount, _panel.HostName, Port, _panel.PlayerName);

      _session?.Dispose();
      try {
        _panel.Visible = false;
        _popup.Visible = true;
        _popup.Text = "Connecting..";

        _session = new ClientSession(cfg, this, new NgisUnityLogger());
      }
      catch (Exception e) {
        _popup.Text = e.Message;
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

    public void JoinedToSession() {
      _popup.Text = "Waiting for players...";
    }

    public void SessionStarted(ServerMsgStart msgStart) {
      _popup.Visible = false;
      _level = new NetworkLevelController(CreateInputSource(), msgStart);
    }

    public void InputReceived(ServerMsgInput msgInput) => _level.InputReceived(msgInput);
    public (Queue<ClientMsgInputs>, ClientMsgFinished?) Process() => _level.Process();

    public void SessionFinished(ServerMsgFinish msgFinish) {
      var frames = string.Join(", ", msgFinish.Frames);
      var hashes = string.Join(", ", msgFinish.Hashes);
      _popup.Visible = true;
      _popup.Text = $"Finished at [{frames}] with state [{hashes}]";
    }

    public void SessionClosedWithError(ClientSessionError errorId, ServerErrorId? serverErrorId = null) {
      _popup.Visible = true;
      _popup.Text = serverErrorId.HasValue ? $"{errorId}: {serverErrorId.Value}" : errorId.ToString();
    }
  }
}