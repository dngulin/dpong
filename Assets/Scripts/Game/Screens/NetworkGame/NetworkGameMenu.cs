using DPong.Game.UI;

namespace DPong.Game.Screens.NetworkGame {
  public interface INetworkGameMenuListener {
    void PlayClicked();
    void BackClicked();
    void NickNameChanged(string name);
    void InputSourceChanged(int srcIndex);
  }

  public class NetworkGameMenu : SimpleMenu {

  }
}