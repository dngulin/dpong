using UnityEngine.InputSystem;

namespace DPong.InputSource {
  public class GamePadInputSource : AbstractButtonInputSource {
    public GamePadInputSource(Gamepad gamePad) : base(gamePad.leftStick.up, gamePad.leftStick.down) {
    }
  }
}