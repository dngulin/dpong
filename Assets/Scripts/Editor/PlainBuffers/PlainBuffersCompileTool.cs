using PlainBuffers;
using PlainBuffers.Generators;
using UnityEditor;
using UnityEngine;

namespace DPong.Editor.PlainBuffers {
  public static class PlainBuffersCompileTool {
    private const string SchemaPath = "Assets/Scripts/Level/State/LevelState.pbs";
    private const string LevelStatePath = "Assets/Scripts/Level/State/LevelState.cs";
    private const string LevelViewStatePath = "Assets/Scripts/Level/View/LevelViewState.cs";

    public static void Compile() {
      CompileLevelState();
      CompileViewState();

      AssetDatabase.Refresh();
    }

    private static void CompileLevelState() {
      Debug.Log("Compile LevelState...");

      var stateCompiler = new PlainBuffersCompiler(
        new CSharpUnityCodeGenerator(new[] {
          "FxNet.Math",
          "FxNet.Random"
        }),
        new[] {
          ExternStructInfo.WithoutValues("FxNum", 8, 8),
          ExternStructInfo.WithEnumeratedValues("FxVec2", 16, 8, new[] {"Zero", "Top", "Left", "Bottom", "Right"}),
          ExternStructInfo.WithoutValues("FxRandomState", 32, 8)
        });

      var (errors, warnings) = stateCompiler.Compile(SchemaPath, LevelStatePath);
      foreach (var warning in warnings)
        Debug.LogWarning(warning);

      foreach (var error in errors)
        Debug.LogError(error);

      if (errors.Length == 0) {
        Debug.Log("LevelState compiled successfully!");
      }
    }

    private static void CompileViewState() {
      Debug.Log("Compile LevelViewState...");

      var stateCompiler = new PlainBuffersCompiler(
        new ViewStateCodeGenerator(new[] {
          "DPong.Level.State",
          "FxNet.Random",
          "UnityEngine"
        }),
        new[] {
          ExternStructInfo.WithoutValues("FxRandomState", 32, 8),
          ExternStructInfo.WithEnumeratedValues("Vector2", 8, 4, new[] {"zero", "top", "left", "bottom", "right"})
        },
        new FixedToFloatTypeMapper());

      var (errors, warnings) = stateCompiler.Compile(SchemaPath, LevelViewStatePath);
      foreach (var warning in warnings)
        Debug.LogWarning(warning);

      foreach (var error in errors)
        Debug.LogError(error);

      if (errors.Length == 0) {
        Debug.Log("LevelViewState compiled successfully!");
      }
    }
  }
}