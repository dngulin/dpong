using System;
using System.Collections.Generic;
using DPong.Game.Navigation;
using DPong.Game.Screens.HotSeatGame;
using DPong.Game.Screens.Main;
using DPong.Game.Screens.NetworkGame;
using DPong.InputSource;
using DPong.Localization;
using DPong.Save;
using DPong.UI;
using UnityEngine;

namespace DPong.Game {
  public class GameCompositionRoot : MonoBehaviour {
    private const string SaveFilename = "save.json";

    [SerializeField] private Canvas _canvas;

    private SaveSystem _save;
    private Navigator _navigator;

    private readonly List<IDisposable> _disposables = new List<IDisposable>();
    private readonly List<ITickable> _tickables = new List<ITickable>();

    private void Awake() {
      if (Application.systemLanguage == SystemLanguage.Russian) {
        Tr.LoadLocale("ru");
      }

      _save = new SaveSystem(SaveFilename);
      _navigator = new Navigator();

      _tickables.Add(_navigator);

      var ui = new UISystem(_canvas);
      var inputs = new InputSourceProvider();

      var hotSeatScreen = new HotSeatGameScreen(_save, inputs, ui, _navigator);
      var netGameScreen = new NetworkGameScreen(_save, inputs, ui, _navigator);

      _disposables.Add(hotSeatScreen);
      _disposables.Add(netGameScreen);

      var hotSeatToken = _navigator.Register(hotSeatScreen);
      var netGameToken = _navigator.Register(netGameScreen);
      var transitions = new MainScreen.Transitions(hotSeatToken, netGameToken);

      _navigator.Enter(new MainScreen(ui, _navigator, transitions));
    }

    private void Update() {
      var dt = Time.deltaTime;
      foreach (var tickable in _tickables)
        tickable.Tick(dt);
    }

    private void OnApplicationFocus(bool hasFocus) {
      if (!hasFocus)
        _save.WriteSaveToFile();
    }

    private void OnApplicationPause(bool pauseStatus) {
      if (pauseStatus)
        _save.WriteSaveToFile();
    }

    private void OnDestroy() {
      _tickables.Clear();

      _disposables.ForEach(d => d.Dispose());
      _disposables.Clear();

      _save.WriteSaveToFile();
      Tr.UnloadLocale();
    }
  }
}