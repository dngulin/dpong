using System;
using DPong.Game.Navigation;
using DPong.Game.Validation;
using DPong.InputSource;
using DPong.InputSource.Extensions;
using DPong.Level;
using DPong.Level.Data;
using DPong.Localization;
using DPong.Save;
using DPong.UI;
using PGM.ScaledNum;
using UnityEngine;

namespace DPong.Game.Screens.HotSeatGame {
  public class HotSeatGameScreen : INavigationPoint, IHotSeatMenuListener, ILevelExitListener, ITickable, IDisposable {
    private readonly Navigator _navigator;
    private readonly SaveSystem _saveSystem;
    private readonly InputSourceProvider _inputSources;

    private readonly UISystem _uiSystem;

    private HotSeatGameMenu _menu;
    private readonly HotSeatGameSave _save;

    private LocalLevelController _levelController;

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

      _levelController?.Dispose();
      _saveSystem.ReturnState(nameof(HotSeatGameScreen), _save);

      if (_menu != null)
        UnityEngine.Object.Destroy(_menu.gameObject);

      _disposed = true;
    }

    void ITickable.FixedTick() => _levelController?.Tick();

    void ITickable.DynamicTick(float dt) {}

    void INavigationPoint.Enter() {
      _menu = _uiSystem.Instantiate(Resources.Load<HotSeatGameMenu>("HotSeatGameMenu"), UILayer.Background, true);
      _menu.Init(this);

      UpdateMenu();
    }

    void INavigationPoint.Suspend() => _menu.Hide();
    void INavigationPoint.Resume() {
      _menu.Show();
      UpdateMenu();
    }

    void INavigationPoint.Exit() {
      _saveSystem.WriteSaveToFile();

      UnityEngine.Object.Destroy(_menu.gameObject);
      _menu = null;
    }

    private void UpdateMenu() {
      _menu.SetPlayerName(Side.Left, _save.LeftName);
      _menu.SetPlayerName(Side.Right, _save.RightName);

      _inputSources.Resfresh();
      _menu.SetInputSources(Side.Left, _inputSources.Names, _inputSources.Descriptors.IndexOf(_save.LeftInput));
      _menu.SetInputSources(Side.Right, _inputSources.Names, _inputSources.Descriptors.IndexOf(_save.RightInput));
    }

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

      var tickDuration = Mathf.RoundToInt(Time.fixedDeltaTime * SnMath.Scale);
      var simSettings = new SimulationSettings(tickDuration, null);

      var levelSettings = new LevelSettings(lPlayer, rPlayer, simSettings);

      var lInput = _inputSources.CreateSource(_save.LeftInput);
      var rInput = _inputSources.CreateSource(_save.RightInput);

      ((INavigationPoint) this).Suspend();
      _levelController = new LocalLevelController(levelSettings, lInput, rInput, _uiSystem, this);
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
      _levelController.Dispose();
      ((INavigationPoint) this).Resume();
    }
  }
}