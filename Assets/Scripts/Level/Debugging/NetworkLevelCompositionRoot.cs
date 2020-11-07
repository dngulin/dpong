using System;
using DPong.InputSource.Sources;
using NGIS.Session.Client;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DPong.Level.Debugging {
  public class NetworkLevelCompositionRoot : MonoBehaviour {
    [SerializeField] private string _hostName;
    [SerializeField] private string _playerName;

    private ClientSession _session;
    private NetworkLevelController _level;

    private void Awake() {
      var cfg = new DPongClientConfig(_hostName, _playerName);
      try {
        _session = new ClientSession(cfg, new NgisUnityLogger());
      }
      catch (Exception e) {
        Debug.LogError(e.Message);
      }
    }

    private void Update() {
      if (_session == null)
        return;

      var result = _session.Process();
      switch (result.Type) {
        case ProcessingResult.ResultType.None:
          break;

        case ProcessingResult.ResultType.Error:
          Debug.LogError($"{result.SessionError} {result.ServerErrorId}");
          break;

        case ProcessingResult.ResultType.Joined:
          Debug.Log("Waiting for players...");
          break;

        case ProcessingResult.ResultType.Started:
          Debug.Log("Session started");
          var inputSource = new KeyboardInputSource(Keyboard.current, Key.W, Key.S);
          _level = new NetworkLevelController(inputSource, result.StartMessage);
          break;

        case ProcessingResult.ResultType.Active:
          var (inputs, optMsgFinish) = _level.Process(_session.ReceivedInputs);
          var optError = _session.SendMessages(inputs, optMsgFinish);
          if (optError.HasValue)
            Debug.LogError(optError.Value);
          break;

        case ProcessingResult.ResultType.Finished:
          var msgFinish = result.FinishMessage;
          var frames = string.Join(", ", msgFinish.Frames);
          var hashes = string.Join(", ", msgFinish.Hashes);
          var (frame, simulations) = _level.SimulationStats;
          Debug.Log($"Session finished at [{frames}] with state [{hashes}]\nSimulations: {frame} / {simulations}");
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void OnDestroy() {
      _session?.Dispose();
      _level?.Dispose();
    }
  }
}