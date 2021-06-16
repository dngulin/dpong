using DPong.Localization;
using DPong.Meta.Screens.MainMenu;
using DPong.StateTracker;
using UnityEngine;

namespace DPong.Meta {
  public class GameCompositionRoot : MonoBehaviour {
    private const string SaveFilename = "save.json";

    [SerializeField] private Canvas _canvas;

    private DPong _dPong;
    private GameStateTracker<DPong> _gameStateTracker;

    private void Awake() {
      if (Application.systemLanguage == SystemLanguage.Russian) {
        Tr.LoadLocale("ru");
      }

      _dPong = new DPong(SaveFilename, _canvas);
      _gameStateTracker = new GameStateTracker<DPong>(_dPong, new MainMenuScreen());
    }

    private void Update() {
      var finished = _gameStateTracker.Tick(Time.deltaTime);
      if (finished)
        ExitGame();
    }

    private void OnApplicationPause(bool pauseStatus) {
      if (pauseStatus)
        _dPong.Save.WriteCaches();
    }

    private void OnDestroy() {
      _dPong.Save.WriteCaches();
      _dPong.Dispose();

      Tr.UnloadLocale();
    }

    private static void ExitGame() {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
  }
}