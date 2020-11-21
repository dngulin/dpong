using DPong.InputSource.Sources;
using DPong.Level.Data;
using DPong.UI;
using PGM.ScaledNum;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DPong.Level.Debugging {
  public class LocalLevelCompositionRoot : MonoBehaviour, ILevelExitListener {

    [SerializeField] private Canvas _canvas;

    [SerializeField] private string _leftName = "LeftPlayer";
    [SerializeField] private bool _leftIsBot;

    [SerializeField] private string _rightName = "RightPlayer";
    [SerializeField] private bool _rightIsBot;

    private LocalLevel _level;

    private void Awake() {
      var levelInfo = CreateLevelInfo();
      var lInputSrc = new KeyboardInputSource(Keyboard.current, Key.W, Key.S);
      var rInputSrc = new KeyboardInputSource(Keyboard.current, Key.UpArrow, Key.DownArrow);

      var uiSystem = new UISystem(_canvas);

      _level = new LocalLevel(levelInfo, lInputSrc, rInputSrc, uiSystem, this);
    }

    private void Update() => _level?.Tick(Time.unscaledDeltaTime);

    void ILevelExitListener.Exit() => Destroy(gameObject);

    private void OnDestroy()
    {
      _level?.Dispose();
    }

    private LevelSettings CreateLevelInfo() {
      var leftPlayer = new PlayerInfo(_leftName, _leftIsBot ? PlayerType.Bot : PlayerType.Local);
      var rightPlayer = new PlayerInfo(_rightName, _rightIsBot ? PlayerType.Bot : PlayerType.Local);

      const int tps = 25;
      var tickDuration = Mathf.RoundToInt((float)SnMath.Scale / tps);
      var settings = new SimulationSettings(tickDuration, null);

      return new LevelSettings(leftPlayer, rightPlayer, settings);
    }
  }
}
