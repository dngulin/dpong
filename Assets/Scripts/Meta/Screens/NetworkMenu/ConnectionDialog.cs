using System;
using DPong.Localization;
using DPong.UI.Holder;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Meta.Screens.NetworkMenu {
  public class ConnectionDialog : UIHolderWrapper {
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Text _message;

    public event Action OnCancelClicked;

    private void Awake() {
      _cancelButton.onClick.AddListener(() => {
        OnCancelClicked?.Invoke();
        Hide();
      });
    }

    public void SetJoinedState(bool joined) {
      _message.text = joined ? Tr._("Waiting for other player...") : Tr._("Connecting to server...");
    }
  }
}