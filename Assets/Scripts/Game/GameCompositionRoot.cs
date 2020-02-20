using DPong.Game.Navigation;
using DPong.Game.Screens;
using DPong.Localization;
using DPong.Save;
using DPong.UI;
using UnityEngine;

namespace DPong.Game {
  public class GameCompositionRoot : MonoBehaviour {
    private const string SaveFilename = "save.json";

    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameResources _resources;

    private SaveSystem _save;
    private UISystem _ui;

    private void Awake() {
      if (Application.systemLanguage == SystemLanguage.Russian) {
        Tr.LoadLocale("ru");
      }

      _save = new SaveSystem(SaveFilename);
      _ui = new UISystem(_resources.UISystemResources, _canvas);

      var navigator = new Navigator();

      var hotSeatToken = navigator.Register(new HotSeatScreen());
      var netGameToken = navigator.Register(new NetGameScreen());

      var transitions = new MainScreenTransitions(hotSeatToken, netGameToken);
      var mainMenu = new MainScreen(_ui, _resources.MainMenuPrefab, navigator, transitions);

      navigator.Enter(mainMenu);
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
      _save.WriteSaveToFile();
      Tr.UnloadLocale();
    }
  }
}