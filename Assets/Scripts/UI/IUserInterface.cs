using System;

namespace DPong.UI {
  public interface IUserInterface  {
    void SetInitialVisibility(bool visible);

    void Show();
    void Hide();

    event Action OnOpenStart;
    event Action OnOpenFinish;
    event Action OnHideStart;
    event Action OnHideFinish;
  }
}