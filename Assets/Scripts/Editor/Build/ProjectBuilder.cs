using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DPong.Editor.Build {
  public static class ProjectBuilder {
    public static void Build() {
      var buildTarget = EditorUserBuildSettings.selectedStandaloneTarget;
      var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

      var manifest = BuildPipeline.BuildAssetBundles(
        Application.streamingAssetsPath,
        BuildAssetBundleOptions.StrictMode | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTarget);

      var bundles = manifest.GetAllAssetBundles();

      var buildOptions = new BuildPlayerOptions {
        scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray(),
        locationPathName = $"Build/{buildTarget}",
        target = buildTarget,
        targetGroup = buildTargetGroup,
        options = BuildOptions.ShowBuiltPlayer
      };
      BuildPipeline.BuildPlayer(buildOptions);

      foreach (var bundleName in bundles.Append("StreamingAssets")) {
        FileUtil.DeleteFileOrDirectory(Path.Combine(Application.streamingAssetsPath, bundleName));
        FileUtil.DeleteFileOrDirectory(Path.Combine(Application.streamingAssetsPath, bundleName + ".meta"));
        FileUtil.DeleteFileOrDirectory(Path.Combine(Application.streamingAssetsPath, bundleName + ".manifest"));
        FileUtil.DeleteFileOrDirectory(Path.Combine(Application.streamingAssetsPath, bundleName + ".manifest.meta"));
      }
    }
  }
}