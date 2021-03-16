using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Localization.Editor {
  public static class PrefabStringExtractor {
    private const string GenerateFileMenu = "Localization/Extract Translatable Strings From Prefabs";

    [MenuItem(GenerateFileMenu)]
    private static void GetLocalizedText() {
      Debug.Log("Collecting strings from prefab...");
      var outputPath = Path.Combine(Application.dataPath, "prefab-extracted-strings.pot");

      const string prefix = "Assets/";

      var entries = new List<LocalizationEntry>();
      var entriesMap = new Dictionary<(string MsgId, string MsgCtx), LocalizationEntry>();

      foreach (var guid in AssetDatabase.FindAssets("t:prefab")) {
        var path = AssetDatabase.GUIDToAssetPath(guid);
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var loaders = asset.GetComponentsInChildren<TranslationLoader>();

        Debug.Log($"Processing prefab: {path}...");
        foreach (var loader in loaders) {
          var msgId = loader.GetComponent<Text>().text;
          var msgCtx = loader.Context ?? string.Empty;
          var msgSrc = $"{path.Substring(prefix.Length)}:{GetObjectPath(loader.transform)}";

          if (entriesMap.TryGetValue((msgId, msgCtx), out var entry)) {
            entry.Sources.Add(msgSrc);
          }
          else {
            entry = new LocalizationEntry(msgSrc, msgId, msgCtx);
            entries.Add(entry);
            entriesMap.Add((msgId, msgCtx), entry);
          }
        }
      }

      Debug.Log($"Writing POT file...");
      using (var fileStream = File.Create(outputPath))
      using (var textWriter = new StreamWriter(fileStream, new UTF8Encoding(false))) {
        foreach (var entry in entries) {
          foreach (var src in entry.Sources)
            textWriter.WriteLine($"#: {src}");

          if (!string.IsNullOrEmpty(entry.Context))
            textWriter.WriteLine($"msgctxt \"{Escape(entry.Context)}\"");

          textWriter.WriteLine($"msgid \"{Escape(entry.MsgId)}\"");
          textWriter.WriteLine("msgstr \"\"\n");
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