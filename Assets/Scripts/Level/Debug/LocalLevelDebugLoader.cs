using DPong.InputSource;
using DPong.Level.Data;
using PGM.ScaledNum;
using UnityEngine;

namespace DPong.Level.Debug {
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
      var left = new KeyBindings { Up = KeyCode.W, Down = KeyCode.S };
      var right = new KeyBindings { Up = KeyCode.P, Down = KeyCode.L };
      return new LocalInputSource(left, right);
    }

    private LevelSettings CreateLevelInfo() {
      var leftPlayer = new PlayerInfo(_leftName, _leftIsBot);
      var rightPlayer = new PlayerInfo(_rightName, _rightIsBot);

      var tickDuration = Mathf.RoundToInt(Time.fixedDeltaTime * SnMath.Scale);
      var settings = new SimulationSettings(tickDuration, null);

      return new LevelSettings(leftPlayer, rightPlayer, settings);
    }
  }
}
