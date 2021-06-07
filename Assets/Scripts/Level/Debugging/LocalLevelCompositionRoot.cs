using System;
using DPong.Assets;
using DPong.InputSource.Sources;
using DPong.Level.Data;
using DPong.UI;
using FxNet.Math;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DPong.Level.Debugging {
  public class LocalLevelCompositionRoot : MonoBehaviour, ILevelExitListener {
    [SerializeField] private Canvas _canvas;

    [SerializeField] private int _simulationFrameRate = 30;

    [SerializeField] private string _leftName = "LeftPlayer";
    [SerializeField] private bool _leftIsBot;

    [SerializeField] private string _rightName = "RightPlayer";
    [SerializeField] private bool _rightIsBot;

    [Range(0, 1)]
    [SerializeField] private float _timeScale = 1.0f;

    private LocalLevel _level;
    private AssetLoader _assetLoader;

    private void Awake() {
      _assetLoader = AssetLoader.Create();

      var viewFactory = new LevelViewFactory(_assetLoader, new UISystem(_assetLoader, _canvas));
      var levelInfo = CreateLevelInfo();

      var lInputSrc = new KeyboardInputSource(Keyboard.current, Key.W, Key.S);
      var rInputSrc = new KeyboardInputSource(Keyboard.current, Key.UpArrow, Key.DownArrow);
      var inputs = new IInputSource[] {lInputSrc, rInputSrc};

      _level = new LocalLevel(levelInfo, inputs, viewFactory, this);
    }

    private void Update() {
      Time.timeScale = _timeScale;
      _level?.Tick(Time.deltaTime);
    }

    void ILevelExitListener.Exit() => Destroy(gameObject);

    private void OnDestroy()
    {
      _level?.Dispose();
      _assetLoader?.Dispose();
    }

    private LevelSettings CreateLevelInfo() {
      var leftPlayer = new PlayerInfo(_leftName, _leftIsBot ? PlayerType.Bot : PlayerType.Local);
      var rightPlayer = new PlayerInfo(_rightName, _rightIsBot ? PlayerType.Bot : PlayerType.Local);
      var settings = new SimulationSettings(FxNum.FromRatio(1, _simulationFrameRate), (ulong) DateTime.UtcNow.Ticks);

      return new LevelSettings(leftPlayer, rightPlayer, settings);
    }
  }
}
