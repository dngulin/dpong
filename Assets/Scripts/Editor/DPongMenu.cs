using DPong.Editor.PlainBuffers;
using UnityEditor;

namespace DPong.Editor {
  public static class DPongMenu {
    [MenuItem("DPong/Compile State")]
    public static void CompileState() => PlainBuffersCompileTool.Compile();
  }
}