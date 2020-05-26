using System;
using DPong.UI.Holder;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Level.UI
{
    public class PausePanel : UIHolder, IDisposable
    {
        public event Action OnCLick;

        [SerializeField] private Button _button;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnCLick?.Invoke());
        }

        public void Dispose()
        {
            if (gameObject != null)
                Destroy(gameObject);
        }
    }
}