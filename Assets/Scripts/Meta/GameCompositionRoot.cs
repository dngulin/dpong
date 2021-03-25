using System;
using System.Collections.Generic;
using DPong.Assets;
using DPong.InputSource;
using DPong.Localization;
using DPong.Meta.Navigation;
using DPong.Meta.Screens.HotSeatGame;
using DPong.Meta.Screens.Main;
using DPong.Meta.Screens.NetworkGame;
using DPong.Save;
using DPong.UI;
using UnityEngine;

namespace DPong.Meta {
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

      var assetLoader = AssetLoader.Create();
      _disposables.Add(assetLoader);

      var ui = new UISystem(assetLoader, _canvas);
      var inputs = new InputSourceProvider();

      var hotSeatScreen = new HotSeatGameScreen(_save, assetLoader, inputs, ui, _navigator);
      var netGameScreen = new NetworkGameScreen(_save, assetLoader, inputs, ui, _navigator);

      _disposables.Add(hotSeatScreen);
      _disposables.Add(netGameScreen);

      var hotSeatToken = _navigator.Register(hotSeatScreen);
      var netGameToken = _navigator.Register(netGameScreen);
      var transitions = new MainScreen.Transitions(hotSeatToken, netGameToken);

      _navigator.Enter(new MainScreen(assetLoader, ui, _navigator, transitions));
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

      for (var i = _disposables.Count - 1; i >= 0; i--)
        _disposables[i].Dispose();
      _disposables.Clear();

      _save.WriteSaveToFile();
      Tr.UnloadLocale();
    }
  }
}