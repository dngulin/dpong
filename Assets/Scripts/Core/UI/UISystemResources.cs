using System.Collections.Generic;
using DPong.Core.UI.Holder;
using UnityEngine;

namespace DPong.Core.UI {
  [CreateAssetMenu(fileName = "UISystemResources", menuName = "DPONG/UISystemResources")]
  public class UISystemResources : ScriptableObject, ISerializationCallbackReceiver {
    [SerializeField] private UIHolder DialogWindow;
    [SerializeField] private UIHolder FullScreenWindow;

    [SerializeField] private UIHolder LeftPanel;
    [SerializeField] private UIHolder RightPanel;

    public Dictionary<WindowType, UIHolder> Windows { get; private set; }
    public Dictionary<PanelType, UIHolder> Panels { get; private set; }

    public void OnBeforeSerialize() {
    }

    public void OnAfterDeserialize() {
      Windows = new Dictionary<WindowType, UIHolder> {
        {WindowType.Dialog, DialogWindow},
        {WindowType.FullScreen, FullScreenWindow}
      };

      Panels = new Dictionary<PanelType, UIHolder> {
        {PanelType.Left, LeftPanel},
        {PanelType.Right, RightPanel}
      };
    }
  }
}