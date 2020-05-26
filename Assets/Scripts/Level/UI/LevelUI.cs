using System;
using DPong.Level.State;
using DPong.Localization;
using DPong.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DPong.Level.UI
{
    public interface ILevelUIListener
    {
        void PauseCLicked();
        void ResumeCLicked();
        void ExitCLicked();
    }

    public class LevelUI : IDisposable
    {
        private readonly LevelUIResources _resources;
        private readonly UISystem _uiSystem;

        private readonly PausePanel _pausePanel;

        private readonly ILevelUIListener _listener;

        public LevelUI(UISystem uiSystem, ILevelUIListener listener)
        {
            _uiSystem = uiSystem;
            _listener = listener;
            _resources = Resources.Load<LevelUIResources>("LevelUIResources");

            _pausePanel = _uiSystem.Instantiate(_resources.pausePanel, UILayer.Background, true);
            _pausePanel.OnCLick += ShowPause;
        }

        private void ShowPause()
        {
            _listener.PauseCLicked();

            var dialog = _uiSystem.CreateInfoBox(false, "Just pause!");
            dialog.Show();
            dialog.OnHideFinish += () =>
            {
                dialog.Destroy();
                _listener.ResumeCLicked(); // TODO: Dispatch resume / exit
            };
        }

        public void ShowResult(HitPointsState hp)
        {
            var msg = Tr._("Game finished with result {0}:{1}");
            var dialog = _uiSystem.CreateInfoBox(false, string.Format(msg, hp.Left, hp.Right));
            dialog.Show();
            dialog.OnHideFinish += () =>
            {
                dialog.Destroy();
                _listener.ExitCLicked();
            };
        }

        public void Dispose()
        {
            if (_pausePanel != null)
                Object.Destroy(_pausePanel.gameObject);
        }
    }
}