using System;
using PlainBuffers;
using UnityEditor;
using UnityEngine;

namespace DPong.Editor.PlainBuffers {
  public static class PlainBuffersCompileTool {
    private const string SchemaPath = "Assets/Scripts/Level/State/LevelState.pbs";
    private const string StatePath = "Assets/Scripts/Level/State/LevelState.cs";

    public static void Compile() {
      var stateCompiler = new PlainBuffersCompiler(
        new UnityCodeGenerator(new[] {
          "FxNet.Math",
          "FxNet.Random"
        }),
        new [] {
          new ExternStructInfo("FxNum", 8, 8, new[] {"FromRaw(0)"}),
          new ExternStructInfo("FxVec2", 16, 8, new[] {"Zero", "One", "Top", "Left", "Bottom", "Right"}),
          new ExternStructInfo("FxRandomState", 32, 8, Array.Empty<string>())
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