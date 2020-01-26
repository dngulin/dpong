using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Localization.Editor {
  public static class UnityStringExtractor {
    private const string GenerateFileMenu = "Localization/Collect Translatable Strings From Prefabs";

    [MenuItem(GenerateFileMenu)]
    private static void GetLocalizedText() {
      Debug.Log("Collecting strings from prefab...");
      var outputPath = Path.Combine(Application.dataPath, "prefab-strings.gettext");

      using (var fileStream = File.Create(outputPath))
      using (var textWriter = new StreamWriter(fileStream, new UTF8Encoding(false))) {
        textWriter.WriteLine("// Auto-generated file! Do not edit!");
        textWriter.WriteLine($"// Execute `{GenerateFileMenu}` to update contents");
        textWriter.WriteLine($"// Generated at {DateTime.Now}\n");

        foreach (var guid in AssetDatabase.FindAssets("t:prefab")) {
          var path = AssetDatabase.GUIDToAssetPath(guid);
          var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
          var loaders = asset.GetComponentsInChildren<LocalizedTextLoader>();

          Debug.Log($"Processing prefab: {path}...");
          foreach (var loader in loaders) {
            var text = loader.GetComponent<Text>().text;
            var escapedText = text.Replace("\"", "\\\"");

            textWriter.Write("\n// ");
            textWriter.WriteLine(path);

            if (string.IsNullOrEmpty(loader.Context)) {
              textWriter.WriteLine($"_(\"{escapedText}\");");
            }
            else {
              var escapedContext = loader.Context.Replace("\"", "\\\"");
              textWriter.WriteLine($"_p(\"{escapedContext}\", \"{escapedText}\");");
            }
          }
        }
      }

      Debug.Log("Done!");
    }
  }
}