using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DPong.Level.Debugging.UI {
  public class DebugUIBase : MonoBehaviour {
    [SerializeField] private Button _button;

    public void SetClickListener(Action onButtonClick) {
      _button.onClick.RemoveAllListeners();
      _button.onClick.AddListener(new UnityAction(onButtonClick));
    }

    public bool Visible {
      set => gameObject.SetActive(value);
    }
  }
}