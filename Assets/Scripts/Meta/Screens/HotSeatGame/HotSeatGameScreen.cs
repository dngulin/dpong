using System;
using DPong.Assets;
using DPong.InputSource;
using DPong.InputSource.Extensions;
using DPong.Level;
using DPong.Level.Data;
using DPong.Localization;
using DPong.Meta.Navigation;
using DPong.Meta.Validation;
using DPong.Save;
using DPong.UI;
using FxNet.Math;

namespace DPong.Meta.Screens.HotSeatGame {
  public class HotSeatGameScreen : INavigationPoint, IHotSeatMenuListener, ILevelExitListener, IDisposable {
    private readonly SaveSystem _saveSystem;
    private readonly AssetLoader _assetLoader;

    private readonly InputSourceProvider _inputSources;

    private readonly Navigator _navigator;
    private readonly UISystem _uiSystem;

    private HotSeatGameMenu _menu;
    private readonly HotSeatGameSave _save;

    private LocalLevel _level;

    private bool _disposed;

    public HotSeatGameScreen(SaveSystem save, AssetLoader assetLoader, InputSourceProvider inputSources, UISystem ui, Navigator navigator) {
      _saveSystem = save;
      _assetLoader = assetLoader;
      _inputSources = inputSources;
      _uiSystem = ui;
      _navigator = navigator;

      _save = _saveSystem.TakeState(nameof(HotSeatGameScreen), new HotSeatGameSave());
    }

    public void Dispose() {
      if (_disposed)
        return;

      _level?.Dispose();
      _saveSystem.ReturnState(nameof(HotSeatGameScreen), _save);

      if (_menu != null)
        UnityEngine.Object.Destroy(_menu.gameObject);

      _disposed = true;
    }

    void INavigationPoint.Tick(float dt) => _level?.Tick(dt);

    void INavigationPoint.Enter() {
      var prefab = _assetLoader.LoadFromPrefab<HotSeatGameMenu>("Assets/Content/Meta/Prefabs/HotSeatGameMenu.prefab");
      _menu = _uiSystem.Instantiate(prefab, UILayer.Background, true);
      _menu.Init(this);
      UpdateMenu();
    }

    void INavigationPoint.Suspend() => HideMenu();
    void INavigationPoint.Resume() => ShowMenu();

    void INavigationPoint.Exit() {
      _saveSystem.WriteSaveToFile();

      UnityEngine.Object.Destroy(_menu.gameObject);
      _menu = null;
    }

    private void ShowMenu() {
      UpdateMenu();
      _menu.Show();
    }

    private void UpdateMenu() {
      _menu.SetPlayerName(Side.Left, _save.LeftName);
      _menu.SetPlayerName(Side.Right, _save.RightName);

      _inputSources.Refresh();
      _menu.SetInputSources(Side.Left, _inputSources.Names, _inputSources.Descriptors.IndexOf(_save.LeftInput));
      _menu.SetInputSources(Side.Right, _inputSources.Names, _inputSources.Descriptors.IndexOf(_save.RightInput));
    }

    private void HideMenu() => _menu.Hide();

    void IHotSeatMenuListener.PlayClicked() {
      if (_save.LeftInput == _save.RightInput)
      {
        var errBox = _uiSystem.CreateErrorBox(false, Tr._("Same control settings are used for both sides"));
        errBox.Show();
        errBox.OnHideFinish += errBox.Destroy;
        return;
      }

      var lPlayer = new PlayerInfo(_save.LeftName, PlayerType.Local);
      var rPlayer = new PlayerInfo(_save.RightName, PlayerType.Local);

      var tickDuration = FxNum.FromRatio(1, 30);
      var simSettings = new SimulationSettings(tickDuration, (ulong) DateTime.UtcNow.Ticks);

      var levelSettings = new LevelSettings(lPlayer, rPlayer, simSettings);

      var lInput = _inputSources.CreateSource(_save.LeftInput);
      var rInput = _inputSources.CreateSource(_save.RightInput);
      var inputs = new [] {lInput, rInput};

      HideMenu();
      var levelViewFactory = new LevelViewFactory(_assetLoader, _uiSystem);
      _level = new LocalLevel(levelSettings, inputs, levelViewFactory, this);
    }

    void IHotSeatMenuListener.BackClicked() => _navigator.Exit(this);

    void IHotSeatMenuListener.NickNameChanged(Side side, string name) {
      var validated = PlayerDataValidator.ValidateNickName(name);
      _menu.SetPlayerName(side, validated);
      (side == Side.Left ? ref _save.LeftName : ref _save.RightName) = validated;
    }

    void IHotSeatMenuListener.InputSourceChanged(Side side, int index) {
      if (index < 0 || index >= _inputSources.Descriptors.Count)
        return;

      (side == Side.Left ? ref _save.LeftInput : ref _save.RightInput) = _inputSources.Descriptors[index];
    }

    void ILevelExitListener.Exit()
    {
      _level.Dispose();
      ShowMenu();
    }
  }
}