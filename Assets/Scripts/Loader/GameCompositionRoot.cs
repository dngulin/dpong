using DPong.Loader.Navigation;
using DPong.Loader.Screens;
using DPong.Localization;
using DPong.Save;
using DPong.UI;
using UnityEngine;

namespace DPong.Loader {
  public class GameCompositionRoot : MonoBehaviour {
    private const string SaveFilename = "save.json";

    [SerializeField] private Canvas _canvas;
    [SerializeField] private MenuPrefabs _menuPrefabs;

    private SaveSystem _save;
    private UISystem _ui;

    private void Awake() {
      if (Application.systemLanguage == SystemLanguage.Russian) {
        Tr.LoadLocale("ru");
      }

      _save = new SaveSystem(SaveFilename);
      _ui = new UISystem(_canvas);

      var navigator = new Navigator();

      var hotSeatToken = navigator.Register(new HotSeatScreen());
      var netGameToken = navigator.Register(new NetGameScreen());

      var transitions = new MainScreenTransitions(hotSeatToken, netGameToken);
      var mainMenu = new MainScreen(_ui, _menuPrefabs, navigator, transitions);

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