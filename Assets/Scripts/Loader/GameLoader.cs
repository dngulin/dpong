using DPong.Loader.UI;
using DPong.Localization;
using DPong.Save;
using DPong.UI;
using UnityEngine;

namespace DPong.Loader {
  public class GameLoader : MonoBehaviour, IMainMenuListener {
    [SerializeField] private Canvas _canvas;

    [SerializeField] private LoaderResources _resources;

    private SaveSystem _saveSystem;
    private UISystem _uiSystem;

    private MainMenu _mainMenu;

    private void Awake() {
      if (Application.systemLanguage == SystemLanguage.Russian) {
        Tr.LoadLocale("ru");
      }

      _saveSystem = new SaveSystem("save.json");
      _uiSystem = new UISystem(_canvas);

      _mainMenu = _uiSystem.Instantiate(_resources.MainMenu, UILayer.Background, true);
      _mainMenu.Init(this);
    }

    private void OnApplicationFocus(bool hasFocus) {
      if (!hasFocus)
        _saveSystem.WriteSaveToFile();
    }

    private void OnApplicationPause(bool pauseStatus) {
      if (pauseStatus)
        _saveSystem.WriteSaveToFile();
    }

    private void OnDestroy() {
      _saveSystem.WriteSaveToFile();
      Tr.UnloadLocale();
    }

    void IMainMenuListener.OnHotSeatClicked() {
      _mainMenu.Hide();
      // TODO: Open hot seat menu
    }

    void IMainMenuListener.OnNetworkClicked() {
      _mainMenu.Show();
      // TODO: Open network menu
    }

    void IMainMenuListener.OnExitGameClicked() {
#if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
  }
}