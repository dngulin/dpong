using DPong.Save;
using DPong.UI;
using UnityEngine;

namespace DPong.Loader {
  public class GameLoader : MonoBehaviour {
    [SerializeField] private Canvas _canvas;

    private SaveSystem _saveSystem;
    private UISystem _uiSystem;

    private void Awake() {
      _saveSystem = new SaveSystem("save.json");
      _uiSystem = new UISystem(_canvas);
    }

    private void OnApplicationFocus(bool hasFocus) {
      if (!hasFocus)
        _saveSystem.WriteSaveToFile();
    }

    private void OnApplicationPause(bool pauseStatus) {
      if (pauseStatus)
        _saveSystem.WriteSaveToFile();
    }
  }
}