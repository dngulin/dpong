using System;
using DPong.Assets;
using DPong.Level.State;
using DPong.Localization;
using DPong.UI;
using DPong.UI.Holder;
using UObj = UnityEngine.Object;

namespace DPong.Level.UI {
  public interface ILevelUIListener {
    void PauseCLicked();
    void ResumeCLicked();
    void ExitCLicked();
  }

  public class LevelUI : IDisposable {
    private readonly LevelUIResources _resources;
    private readonly UISystem _uiSystem;

    private readonly PausePanel _pausePanel;

    private DisplayingDialogType _displayingDialogType;
    private UIHolderWrapper _displayingDialog;

    private readonly ILevelUIListener _listener;

    public LevelUI(AssetLoader assetLoader, UISystem uiSystem, ILevelUIListener listener) {
      _uiSystem = uiSystem;
      _listener = listener;
      _resources = assetLoader.Load<LevelUIResources>("Assets/Content/Level/LevelUIResources.asset");

      _pausePanel = _uiSystem.Instantiate(_resources.PausePanel, UILayer.Background, true);
      _pausePanel.OnCLick += ShowPauseDialog;
    }

    private void ShowPauseDialog() {
      if (_displayingDialogType != DisplayingDialogType.None)
        throw new InvalidOperationException();

      _displayingDialogType = DisplayingDialogType.Pause;

      _listener.PauseCLicked();

      var dialog = _uiSystem.InstantiateWindow(WindowType.Dialog, _resources.PauseDialog, false);
      _displayingDialog = dialog;

      dialog.Show();
      dialog.OnHideFinish += () => {
        ClearDisplayingDialog();
        switch (dialog.Result) {
          case PauseDialog.Intent.Resume:
            _listener.ResumeCLicked();
            break;
          case PauseDialog.Intent.Exit:
            _listener.ExitCLicked();
            break;
        }
      };
    }

    public void ShowResultDialog(in ScoresState hp) {
      switch (_displayingDialogType) {
        case DisplayingDialogType.None:
          break;

        case DisplayingDialogType.Pause:
        case DisplayingDialogType.Error:
          ClearDisplayingDialog();
          break;

        default:
          throw new InvalidOperationException();
      }

      _displayingDialogType = DisplayingDialogType.Result;

      var msg = Tr._("Game finished with result {0}:{1}");
      var dialog = _uiSystem.CreateInfoBox(false, string.Format(msg, hp.Left, hp.Right));
      _displayingDialog = dialog;

      dialog.Show();
      dialog.OnHideFinish += () => {
        ClearDisplayingDialog();
        _listener.ExitCLicked();
      };
    }

    public void ShowErrorDialog(string error) {
      switch (_displayingDialogType) {
        case DisplayingDialogType.None:
        case DisplayingDialogType.Result:
          break;

        case DisplayingDialogType.Pause:
          ClearDisplayingDialog();
          break;

        default:
          throw new InvalidOperationException();
      }

      var dialog = _uiSystem.CreateErrorBox(false, error);
      _displayingDialog = dialog;

      dialog.Show();
      dialog.OnHideFinish += () => {
        ClearDisplayingDialog();
        _listener.ExitCLicked();
      };
    }

    private void ClearDisplayingDialog() {
      _displayingDialog.Destroy();
      _displayingDialog = null;
      _displayingDialogType = DisplayingDialogType.None;
    }

    public void Dispose() {
      if (_pausePanel != null)
        UObj.Destroy(_pausePanel.gameObject);

      if (_displayingDialog != null)
        _displayingDialog.Destroy();

      _displayingDialog = null;
      _displayingDialogType = DisplayingDialogType.None;
    }
  }
}