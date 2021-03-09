using System;
using DPong.InputSource.Sources;
using DPong.Level.Data;
using DPong.UI;
using FxNet.Math;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DPong.Level.Debugging {
  public class LocalLevelCompositionRoot : MonoBehaviour, ILevelExitListener {

    [SerializeField] private Canvas _canvas;

    [SerializeField] private string _leftName = "LeftPlayer";
    [SerializeField] private bool _leftIsBot;

    [SerializeField] private string _rightName = "RightPlayer";
    [SerializeField] private bool _rightIsBot;

    [Range(0, 1)]
    [SerializeField] private float _timeScale = 1.0f;

    private LocalLevel _level;

    private void Awake() {
      var levelInfo = CreateLevelInfo();
      var lInputSrc = new KeyboardInputSource(Keyboard.current, Key.W, Key.S);
      var rInputSrc = new KeyboardInputSource(Keyboard.current, Key.UpArrow, Key.DownArrow);

      var uiSystem = new UISystem(_canvas);

      _level = new LocalLevel(levelInfo, lInputSrc, rInputSrc, uiSystem, this);
    }

    private void Update() {
      Time.timeScale = _timeScale;
      _level?.Tick(Time.deltaTime);
    }

    void ILevelExitListener.Exit() => Destroy(gameObject);

    private void OnDestroy()
    {
      _level?.Dispose();
    }

    private LevelSettings CreateLevelInfo() {
      var leftPlayer = new PlayerInfo(_leftName, _leftIsBot ? PlayerType.Bot : PlayerType.Local);
      var rightPlayer = new PlayerInfo(_rightName, _rightIsBot ? PlayerType.Bot : PlayerType.Local);
      var settings = new SimulationSettings(FxNum.FromRatio(1, 25), (ulong) DateTime.UtcNow.Ticks);

      return new LevelSettings(leftPlayer, rightPlayer, settings);
    }
  }
}
