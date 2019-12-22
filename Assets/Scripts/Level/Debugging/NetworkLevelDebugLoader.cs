using DPong.InputSource;
using NGIS.Session.Client;
using UnityEngine;

namespace DPong.Level.Debugging {
  public class NetworkLevelDebugLoader : MonoBehaviour {
    private const string GameName = "dPONG";
    private const byte PlayerCount = 2;

    [SerializeField] private string _playerName = "Player";

    [SerializeField] private string _host = "localhost";
    [SerializeField] private int _port = 5081;

    [SerializeField] private ushort _gameVersion;

    private ClientSession _session;

    private void Awake() {
      var cfg = new ClientConfig(GameName, _gameVersion, PlayerCount, _host, _port, _playerName);
      var client = new NetworkLevelController(CreateInputSource());

      _session = new ClientSession(cfg, client);
    }

    private void Update() {
      _session?.Process();
    }

    private void OnDestroy() {
      _session?.Dispose();
    }

    private static ILocalInputSource CreateInputSource() {
      var left = new KeyBindings { Up = KeyCode.W, Down = KeyCode.S };
      var right = new KeyBindings { Up = KeyCode.P, Down = KeyCode.L };
      return new LocalInputSource(left, right);
    }
  }
}