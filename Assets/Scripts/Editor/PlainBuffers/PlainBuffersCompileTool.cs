using PlainBuffers;
using PlainBuffers.Generators;
using UnityEditor;
using UnityEngine;

namespace DPong.Editor.PlainBuffers {
  public static class PlainBuffersCompileTool {
    private const string SchemaPath = "Assets/Scripts/Level/State/LevelState.pbs";
    private const string StatePath = "Assets/Scripts/Level/State/LevelState.cs";

    public static void Compile() {
      var stateCompiler = new PlainBuffersCompiler(
        new CSharpUnityCodeGenerator(new[] {
          "FxNet.Math",
          "FxNet.Random"
        }),
        new [] {
          ExternStructInfo.WithoutValues("FxNum", 8, 8),
          ExternStructInfo.WithEnumeratedValues("FxVec2", 16, 8, new[] {"Zero", "Top", "Left", "Bottom", "Right"}),
          ExternStructInfo.WithoutValues("FxRandomState", 32, 8)
        });

      var (errors, warnings) = stateCompiler.Compile(SchemaPath, StatePath);
      foreach (var warning in warnings)
        Debug.LogWarning(warning);

      foreach (var error in errors)
        Debug.LogError(error);

      if (errors.Length == 0)
        Debug.Log("State compiled successfully!");

      AssetDatabase.Refresh();
    }
  }
}