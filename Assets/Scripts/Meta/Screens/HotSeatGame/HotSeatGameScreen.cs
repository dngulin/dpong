using System;
using DPong.InputSource;
using DPong.InputSource.Extensions;
using DPong.Level;
using DPong.Level.Data;
using DPong.Localization;
using DPong.Meta.Navigation;
using DPong.Meta.Validation;
using DPong.Save;
using DPong.UI;
using PGM.ScaledNum;
using UnityEngine;

namespace DPong.Meta.Screens.HotSeatGame {
  public class HotSeatGameScreen : INavigationPoint, IHotSeatMenuListener, ILevelExitListener, IDisposable {
    private readonly Navigator _navigator;
    private readonly SaveSystem _saveSystem;
    private readonly InputSourceProvider _inputSources;

    private readonly UISystem _uiSystem;

    private HotSeatGameMenu _menu;
    private readonly HotSeatGameSave _save;

    private LocalLevel _level;

    private bool _disposed;

    public HotSeatGameScreen(SaveSystem save, InputSourceProvider inputSources, UISystem ui, Navigator navigator) {
      _saveSystem = save;
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
      _menu = _uiSystem.Instantiate(Resources.Load<HotSeatGameMenu>("HotSeatGameMenu"), UILayer.Background, true);
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

      var tickDuration = Mathf.RoundToInt(1f / 30 * SnMath.Scale);
      var simSettings = new SimulationSettings(tickDuration, null);

      var levelSettings = new LevelSettings(lPlayer, rPlayer, simSettings);

      var lInput = _inputSources.CreateSource(_save.LeftInput);
      var rInput = _inputSources.CreateSource(_save.RightInput);

      HideMenu();
      _level = new LocalLevel(levelSettings, lInput, rInput, _uiSystem, this);
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