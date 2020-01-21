using UnityEngine;

namespace DPong.Core.UI.Holder {
  public abstract class UIHolderContent : MonoBehaviour {
    internal void InternalInit(UIHolder holder) => Holder = holder;
    public UIHolder Holder { get; private set; }
  }
}