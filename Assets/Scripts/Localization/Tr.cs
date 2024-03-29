using System.IO;
using NGettext;
using UnityEngine;

namespace DPong.Localization {
  public static class Tr {
    private static Catalog _catalog;

    public static void LoadLocale(string localeName) {
      var moFilePath = Path.Combine(Application.streamingAssetsPath, "Locale", $"{localeName}.mo");
      using (var moFileStream = File.OpenRead(moFilePath)) {
        _catalog = new Catalog(moFileStream);
      }
    }

    public static void UnloadLocale() {
      _catalog = null;
    }

    public static string _(string text) {
      return _catalog?.GetString(text) ?? text;
    }

    public static string _n(string single, string plural, int n) {
      return _catalog?.GetPluralString(single, plural, n) ?? (n == 1 ? single : plural);
    }

    public static string _p(string ctx, string text) {
      return _catalog?.GetParticularString(ctx, text) ?? text;
    }

    public static string _pn(string ctx, string single, string plural, int n) {
      return _catalog?.GetParticularPluralString(ctx, single, plural, n) ?? (n == 1 ? single : plural);
    }
  }
}