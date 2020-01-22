using System;
using DPong.Core.UI.Holder;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DPong.Level.Debugging.UI {
  public class DbgPopup : UIHolderWrapper {
    [SerializeField] private Text _message;
    [SerializeField] private Button _button;

    public void Init(string text, Action onButtonClick) {
      _button.onClick.RemoveAllListeners();
      _button.onClick.AddListener(new UnityAction(onButtonClick));
      _message.text = text;
    }

    public void UpdateMessage(string text) {
      _message.text = text;
    }
  }
}