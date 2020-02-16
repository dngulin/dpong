using UnityEngine;
using UnityEngine.InputSystem;

namespace DPong.InputSource {
#if UNITY_EDITOR
  [UnityEditor.InitializeOnLoad]
#endif
  public class EditorPointerPositionCrutch : InputProcessor<Vector2> {
#if UNITY_EDITOR
    private static readonly System.Type GameViewType;
    private static readonly bool EditorScaled;
#endif

#if UNITY_EDITOR
    static EditorPointerPositionCrutch() {
      EditorScaled = System.Environment.GetEnvironmentVariable("GDK_SCALE") == "2";

      var assembly = typeof(UnityEditor.EditorWindow).Assembly;
      GameViewType = assembly.GetType("UnityEditor.GameView");

      Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize() {
      InputSystem.RegisterProcessor<EditorPointerPositionCrutch>();
    }

    // Workaround for https://github.com/Unity-Technologies/InputSystem/issues/1025
    // Processing works correctly only with fixed resolution and GameView scale = 1
    public override Vector2 Process(Vector2 value, InputControl control) {
#if UNITY_EDITOR_LINUX
      if (EditorScaled) {
        var window = UnityEditor.EditorWindow.GetWindow(GameViewType);
        var w = window.position.width * 2;
        var h = (window.position.height - 20) * 2; // ignore top panel height
        var shift = new Vector2((w - Screen.width) / 2, (h - Screen.height) / 2);

        value.y -= 400; // bottom gameView's point has y = 400
        value = value * 2 - shift;
      }
#endif

      return value;
    }
  }
}