using System;
using DPong.Assets;
using DPong.InputSource;
using DPong.Save;
using DPong.UI;
using UnityEngine;

namespace DPong.Meta {
  public class DPong : IDisposable {
    public readonly AssetLoader Assets;
    public readonly SaveSystem Save;
    public readonly UiSystem Ui;
    public readonly InputSourceProvider InputSources;

    public DPong(string saveFileName, Canvas uiCanvas) {
      Assets = AssetLoader.Create();
      Save = new SaveSystem(saveFileName);
      Ui = new UiSystem(Assets, uiCanvas);
      InputSources = new InputSourceProvider();
    }

    public void Dispose() {
      Assets.Dispose();
    }
  }
}