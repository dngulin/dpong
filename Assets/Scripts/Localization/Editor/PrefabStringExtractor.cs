using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Localization.Editor {
  public static class PrefabStringExtractor {
    private const string GenerateFileMenu = "Localization/Collect Translatable Strings From Prefabs";

    [MenuItem(GenerateFileMenu)]
    private static void GetLocalizedText() {
      Debug.Log("Collecting strings from prefab...");
      var outputPath = Path.Combine(Application.dataPath, "prefab-extracted-strings.pot");

      using (var fileStream = File.Create(outputPath))
      using (var textWriter = new StreamWriter(fileStream, new UTF8Encoding(false))) {
        foreach (var guid in AssetDatabase.FindAssets("t:prefab")) {
          var path = AssetDatabase.GUIDToAssetPath(guid);
          var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
          var loaders = asset.GetComponentsInChildren<TranslationLoader>();

          Debug.Log($"Processing prefab: {path}...");
          foreach (var loader in loaders) {
            textWriter.WriteLine($"#: {path}/{GetObjectPath(loader.transform)}");

            if (!string.IsNullOrEmpty(loader.Context))
              textWriter.WriteLine($"msgctxt \"{Escape(loader.Context)}\"");

            var text = loader.GetComponent<Text>().text;
            textWriter.WriteLine($"msgid \"{Escape(text)}\"");
            textWriter.WriteLine("msgstr \"\"\n");
          }
        }
      }

      Debug.Log("Done!");
    }

    private static string GetObjectPath(Transform obj) {
      var path = obj.name;

      while (obj.parent != null) {
        obj = obj.parent;
        path = $"{obj.name}/{path}";
      }

      return path;
    }

    private static string Escape(string text) {
      return text
        .Replace("\\", "\\\\")
        .Replace("\"", "\\\"")
        .Replace("\n", "\\n");
    }
  }
}