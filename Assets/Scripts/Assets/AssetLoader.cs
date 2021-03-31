using System;
using System.IO;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace DPong.Assets {
  public abstract class AssetLoader : IDisposable {
    public abstract T Load<T>(string name) where T : UObj;
    public abstract void Dispose();

    public static AssetLoader Create() {
#if UNITY_EDITOR
      return new EditorAssetLoader();
#else
      return new PlayerAssetLoader();
#endif
    }
  }

  internal class PlayerAssetLoader : AssetLoader {
    private readonly AssetBundle _bundle;

    public PlayerAssetLoader() {
      _bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "dpong"));
    }

    public override T Load<T>(string name) => _bundle.LoadAsset<T>(name);
    public override void Dispose() => _bundle.Unload(true);
  }

#if UNITY_EDITOR
  internal class EditorAssetLoader : AssetLoader {
    public override T Load<T>(string name) => UnityEditor.AssetDatabase.LoadAssetAtPath<T>(name);

    public override void Dispose() {
    }
  }
#endif
}