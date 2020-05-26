using System;
using DPong.UI.Common;
using DPong.UI.Holder;
using UnityEngine;

namespace DPong.UI {
  [CreateAssetMenu(fileName = "UISystemResources", menuName = "DPong/UISystemResources")]
  public class UISystemResources : ScriptableObject {
    [SerializeField] private UIHolder _dialogHolder;
    [SerializeField] private UIHolder _fullScreenHolder;

    public MessageBox MsgBox;

    public UIHolder this[WindowType windowType] {
      get {
        switch (windowType) {
          case WindowType.Dialog:
            return _dialogHolder;

          case WindowType.FullScreen:
            return _fullScreenHolder;

          default:
            throw new ArgumentOutOfRangeException(nameof(windowType), windowType, null);
        }
      }
    }
  }
}