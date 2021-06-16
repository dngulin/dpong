using System;
using System.Collections.Generic;
using DPong.InputSource;
using DPong.InputSource.Extensions;
using DPong.Level.Data;
using DPong.Localization;
using DPong.Meta.Screens.HotSeatGame;
using DPong.Meta.Validation;
using DPong.Save;
using DPong.StateTracker;
using DPong.UI;
using FxNet.Math;
using UnityEngine;

namespace DPong.Meta.Screens.HotSeatMenu {
  public class HotSeatMenuScreen : GameState<DPong>, IHotSeatMenuListener {
    private readonly HotSeatGameSave _save;

    private HotSeatGameMenu _menu;
    private readonly Queue<Intent> _intents = new Queue<Intent>();

    public HotSeatMenuScreen(SaveSystem saveSystem) {
      _save = saveSystem.TakeState(nameof(HotSeatMenuScreen), new HotSeatGameSave());
    }

    void IHotSeatMenuListener.PlayClicked() => _intents.Enqueue(Intent.Play());
    void IHotSeatMenuListener.BackClicked() => _intents.Enqueue(Intent.Back());
    void IHotSeatMenuListener.NickNameChanged(Side side, string nick) => _intents.Enqueue(Intent.NickChanged(side, nick));
    void IHotSeatMenuListener.InputSrcChanged(Side side, int index) => _intents.Enqueue(Intent.InputSrcChanged(side, index));

    public override void Start(DPong game) {
      var prefab = game.Assets.LoadFromPrefab<HotSeatGameMenu>("Assets/Content/Meta/Prefabs/HotSeatGameMenu.prefab");
      _menu = game.Ui.Instantiate(prefab, UILayer.Background, true);
      _menu.Init(this);
      UpdateMenu(game.InputSources);
    }

    public override Transition Tick(DPong game, float dt) {
      var trans = Transition.None();

      while (_intents.Count > 0) {
        var intent = _intents.Dequeue();

        switch (intent.Type) {
          case IntentType.Back: {
            Debug.Assert(trans.Type == TransitionType.None, $"Transition {trans.Type} -> {TransitionType.Pop}");
            trans = Transition.Pop();
            break;
          }

          case IntentType.Play: {
            if (_save.LeftInput != _save.RightInput) {
              Debug.Assert(trans.Type == TransitionType.None, $"Transition {trans.Type} -> {TransitionType.Push}");
              trans = Transition.Push(BuildHotSeatGameScreen(game));
            }
            else {
              ShowPlayParamsError(game.Ui);
            }

            break;
          }

          case IntentType.NickChanged: {
            var (side, name) = intent.GetNickChangedData();
            var validatedNick = PlayerDataValidator.ValidateNickName(name);
            _menu.SetPlayerName(side, validatedNick);
            (side == Side.Left ? ref _save.LeftName : ref _save.RightName) = validatedNick;
            break;
          }

          case IntentType.InputSrcChanged: {
            var (side, index) = intent.GetInputSourceChangedData();
            (side == Side.Left ? ref _save.LeftInput : ref _save.RightInput) = game.InputSources.Descriptors[index];
            break;
          }

          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      return trans;
    }

    private HotSeatGameScreen BuildHotSeatGameScreen(DPong game) {
      var tickDuration = FxNum.FromRatio(1, 60);
      var simSettings = new SimulationSettings(tickDuration, (ulong) DateTime.UtcNow.Ticks);

      var lPlayer = new PlayerInfo(_save.LeftName, PlayerType.Local);
      var rPlayer = new PlayerInfo(_save.RightName, PlayerType.Local);
      var levelSettings = new LevelSettings(lPlayer, rPlayer, simSettings);

      var lInput = game.InputSources.CreateSource(_save.LeftInput);
      var rInput = game.InputSources.CreateSource(_save.RightInput);
      var inputSources = new [] {lInput, rInput};

      return new HotSeatGameScreen(levelSettings, inputSources, game);
    }

    private static void ShowPlayParamsError(UiSystem uiSystem) {
      var errBox = uiSystem.CreateErrorBox(false, Tr._("Same control settings are used for both sides"));
      errBox.Show();
      errBox.OnHideFinish += errBox.Destroy;
    }

    public override void Pause(DPong game) => _menu.Hide();
    public override void Resume(DPong game) => ShowMenu(game.InputSources);

    public override void Finish(DPong game) {
      game.Save.WriteCaches();
      game.Save.ReturnState(nameof(HotSeatMenuScreen), _save);

      UnityEngine.Object.Destroy(_menu.gameObject);
      _menu = null;
    }

    private void ShowMenu(InputSourceProvider inputSources) {
      UpdateMenu(inputSources);
      _menu.Show();
    }

    private void UpdateMenu(InputSourceProvider inputSources) {
      _menu.SetPlayerName(Side.Left, _save.LeftName);
      _menu.SetPlayerName(Side.Right, _save.RightName);

      inputSources.Refresh();
      _menu.SetInputSources(Side.Left, inputSources.Names, inputSources.Descriptors.IndexOf(_save.LeftInput));
      _menu.SetInputSources(Side.Right, inputSources.Names, inputSources.Descriptors.IndexOf(_save.RightInput));
    }
  }
}