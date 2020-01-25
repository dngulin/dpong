using DPong.InputSource;
using DPong.Level.Data;
using PGM.ScaledNum;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DPong.Level.Debugging {
  public class LocalLevelDebugLoader : MonoBehaviour {

    [SerializeField] private string _leftName = "LeftPlayer";
    [SerializeField] private bool _leftIsBot;

    [SerializeField] private string _rightName = "RightPlayer";
    [SerializeField] private bool _rightIsBot;

    private LocalLevelController _levelController;

    private void Awake() {
      var levelInfo = CreateLevelInfo();
      var inputSource = CreateInputSource();

      _levelController = new LocalLevelController(levelInfo, inputSource);
    }

    private void FixedUpdate() => _levelController?.Tick();

    private static ILocalInputSource CreateInputSource() {
      var left = new KeyBindings { Up = Key.W, Down = Key.S };
      var right = new KeyBindings { Up = Key.P, Down = Key.L };
      return new KeyboardInputSource(left, right);
    }

    private LevelSettings CreateLevelInfo() {
      var leftPlayer = new PlayerInfo(_leftName, _leftIsBot ? PlayerType.Bot : PlayerType.Local);
      var rightPlayer = new PlayerInfo(_rightName, _rightIsBot ? PlayerType.Bot : PlayerType.Local);

      var tickDuration = Mathf.RoundToInt(Time.fixedDeltaTime * SnMath.Scale);
      var settings = new SimulationSettings(tickDuration, null);

      return new LevelSettings(leftPlayer, rightPlayer, settings);
    }
  }
}
