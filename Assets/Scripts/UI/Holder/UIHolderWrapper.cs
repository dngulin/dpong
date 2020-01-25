using System;
using UnityEngine;

namespace DPong.UI.Holder {
  public abstract class UIHolderWrapper : MonoBehaviour {
    internal void InternalInit(UIHolder holder) => _holder = holder;
    private UIHolder _holder;

    public event Action OnOpenStart {
      add => _holder.OnOpenStart += value;
      remove => _holder.OnOpenStart -= value;
    }

    public event Action OnOpenFinish {
      add => _holder.OnOpenFinish += value;
      remove => _holder.OnOpenFinish -= value;
    }

    public event Action OnHideStart {
      add => _holder.OnHideStart += value;
      remove => _holder.OnHideStart -= value;
    }

    public event Action OnHideFinish {
      add => _holder.OnHideFinish += value;
      remove => _holder.OnHideFinish -= value;
    }

    public void Show() => _holder.Show();
    public void Hide() => _holder.Hide();

    public void Destroy() => Destroy(_holder.gameObject);
  }
}