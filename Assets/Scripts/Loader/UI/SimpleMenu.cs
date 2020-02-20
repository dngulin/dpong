using System;
using DPong.UI;
using UnityEngine;

namespace DPong.Loader.UI {
  public abstract class SimpleMenu : MonoBehaviour, IUserInterface {
    public void SetInitialVisibility(bool visible) => gameObject.SetActive(visible);

    public void Show() {
      if (gameObject.activeSelf) return;
      gameObject.SetActive(true);

      OnOpenStart?.Invoke();
      OnOpenFinish?.Invoke();
    }

    public void Hide() {
      if (!gameObject.activeSelf) return;
      gameObject.SetActive(false);

      OnHideStart?.Invoke();
      OnHideFinish?.Invoke();
    }

    public event Action OnOpenStart;
    public event Action OnOpenFinish;
    public event Action OnHideStart;
    public event Action OnHideFinish;
  }
}