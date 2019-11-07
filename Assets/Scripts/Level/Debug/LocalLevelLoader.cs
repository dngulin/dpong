using DPong.InputSource;
using DPong.Level.Data;
using PGM.ScaledNum;
using UnityEngine;

namespace DPong.Level.Debug {
  public class LocalLevelLoader : MonoBehaviour {

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

    private LevelInfo CreateLevelInfo() {
      var leftPlayer = new PlayerInfo(_leftName, _leftIsBot);
      var rightPlayer = new PlayerInfo(_rightName, _rightIsBot);

      var tickTime = Mathf.RoundToInt(Time.fixedDeltaTime * SnMath.Scale);
      var settings = new SimulationSettings(tickTime, null);

      return new LevelInfo(leftPlayer, rightPlayer, settings);
    }

    /*private void OnDrawGizmos() {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireCube(Vector3.zero, new Vector3(30, 20, 0));

      Gizmos.color = Color.green;
      var left = _state.LeftBlocker;
      var right = _state.RightBlocker;
      Gizmos.DrawWireCube(left.Pose.Position.ToUnityVector(), left.Size.AsRect.ToUnityVector());
      Gizmos.DrawWireCube(right.Pose.Position.ToUnityVector(), right.Size.AsRect.ToUnityVector());

      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(_state.Ball.Pose.Position.ToUnityVector(), 0.5f);
    }*/
  }
}
