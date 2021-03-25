using DPong.Assets;
using DPong.Level.Data;
using DPong.Level.State;
using DPong.Level.UI;
using DPong.Level.View;
using DPong.UI;

namespace DPong.Level {
  public readonly struct LevelViewFactory {
    private readonly AssetLoader _assetLoader;
    private readonly UISystem _uiSystem;

    public LevelViewFactory(AssetLoader assetLoader, UISystem uiSystem) {
      _assetLoader = assetLoader;
      _uiSystem = uiSystem;
    }

    public LevelView CreateView(in LevelState initialState, LevelSettings settings) {
      return new LevelView(_assetLoader, initialState, settings);
    }

    public LevelUI CreateUI(ILevelUIListener listener) {
      return new LevelUI(_assetLoader, _uiSystem, listener);
    }
  }
}