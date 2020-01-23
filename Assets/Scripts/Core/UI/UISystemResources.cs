using System;
using DPong.Core.UI.Holder;
using UnityEngine;

namespace DPong.Core.UI {
  [CreateAssetMenu(fileName = "UISystemResources", menuName = "DPONG/UISystemResources")]
  public class UISystemResources : ScriptableObject {
    [SerializeField] private UIHolder _dialogHolder;
    [SerializeField] private UIHolder _fullScreenHolder;

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