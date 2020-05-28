using DPong.UI.Holder;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Level.UI
{
    public class PauseDialog : UIHolderWrapper
    {
        public enum Intent
        {
            Resume,
            Exit
        }

        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _exitButton;

        private void Awake()
        {
            _resumeButton.onClick.AddListener(() =>
            {
                Result = Intent.Resume;
                Hide();
            });
            _exitButton.onClick.AddListener(() =>
            {
                Result = Intent.Exit;
                Hide();
            });
        }

        public Intent Result { get; private set; }
    }
}