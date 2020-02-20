﻿using DPong.InputSource;
using DPong.Level.Data;
using PGM.ScaledNum;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DPong.Level.Debugging {
  public class LocalLevelCompositionRoot : MonoBehaviour {

    [SerializeField] private string _leftName = "LeftPlayer";
    [SerializeField] private bool _leftIsBot;

    [SerializeField] private string _rightName = "RightPlayer";
    [SerializeField] private bool _rightIsBot;

    private LocalLevelController _levelController;

    private void Awake() {
      var levelInfo = CreateLevelInfo();
      var lInputSrc = new KeyboardInputSource(Keyboard.current, Key.W, Key.S);
      var rInputSrc = new KeyboardInputSource(Keyboard.current, Key.UpArrow, Key.DownArrow);

      _levelController = new LocalLevelController(levelInfo, lInputSrc, rInputSrc);
    }

    private void FixedUpdate() => _levelController?.Tick();

    private LevelSettings CreateLevelInfo() {
      var leftPlayer = new PlayerInfo(_leftName, _leftIsBot ? PlayerType.Bot : PlayerType.Local);
      var rightPlayer = new PlayerInfo(_rightName, _rightIsBot ? PlayerType.Bot : PlayerType.Local);

      var tickDuration = Mathf.RoundToInt(Time.fixedDeltaTime * SnMath.Scale);
      var settings = new SimulationSettings(tickDuration, null);

      return new LevelSettings(leftPlayer, rightPlayer, settings);
    }
  }
}