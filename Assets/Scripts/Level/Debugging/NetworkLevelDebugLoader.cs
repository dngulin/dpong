using System;
using System.Collections.Generic;
using DPong.InputSource;
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

    [SerializeField] private string _hostName;
    [SerializeField] private string _playerName;

    private ClientSession _session;
    private NetworkLevelController _level;

    private void Awake() {
      var cfg = new ClientConfig(GameName, Version, PlayerCount, _hostName, Port, _playerName);
      try {
        _session = new ClientSession(cfg, this, new NgisUnityLogger());
      }
      catch (Exception e) {
        Debug.LogError(e.Message);
      }
    }

    private void Update() {
      _session?.Process();
    }

    private void OnDestroy() {
      _session?.Dispose();
      _level?.Dispose();
    }

    public void JoinedToSession() => Debug.Log("Waiting for players...");

    public void SessionStarted(ServerMsgStart msgStart) {
      var inputSource = new KeyboardInputSource(Keyboard.current, Key.W, Key.S);
      _level = new NetworkLevelController(inputSource, msgStart);
    }

    public void InputReceived(ServerMsgInput msgInput) => _level.InputReceived(msgInput);
    public (Queue<ClientMsgInputs>, ClientMsgFinished?) Process() => _level.Process();

    public void SessionFinished(ServerMsgFinish msgFinish) {
      var frames = string.Join(", ", msgFinish.Frames);
      var hashes = string.Join(", ", msgFinish.Hashes);
      var (frame, simulations) = _level.SimulationStats;

      Debug.Log($"Finished at [{frames}] with state [{hashes}]\nSimulations: {frame} / {simulations}");
    }

    public void SessionClosedWithError(ClientSessionError errorId, ServerErrorId? serverErrorId = null) {
      Debug.LogError(serverErrorId.HasValue ? $"{errorId}: {serverErrorId.Value}" : errorId.ToString());
    }
  }
}