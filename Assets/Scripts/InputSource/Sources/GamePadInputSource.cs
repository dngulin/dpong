using UnityEngine.InputSystem;

namespace DPong.InputSource.Sources {
  public class GamePadInputSource : AbstractButtonInputSource {
    public GamePadInputSource(Gamepad gamePad) : base(gamePad.leftStick.up, gamePad.leftStick.down) {
    }
  }
}