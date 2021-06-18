using System;
using DPong.InputSource;
using DPong.InputSource.Extensions;
using DPong.Level;
using DPong.Localization;
using DPong.Meta.Screens.WaitingNetwork;
using DPong.Meta.Validation;
using DPong.Save;
using DPong.StateTracker;
using DPong.UI;
using NGIS.Session.Client;

namespace DPong.Meta.Screens.NetworkMenu {
  public class NetworkMenuScreen : GameState<DPong> {
    private readonly NetworkGameSave _save;
    private NetworkGameMenu _menu;

    public NetworkMenuScreen(SaveSystem saveSystem) {
      _save = saveSystem.TakeState(nameof(NetworkMenuScreen), new NetworkGameSave());
    }

    public override void Start(DPong game) {
      var prefab = game.Assets.LoadFromPrefab<NetworkGameMenu>("Assets/Content/Meta/Prefabs/NetworkGameMenu.prefab");
      _menu = game.Ui.Instantiate(prefab, UILayer.Background, true);
      UpdateMenu(game.InputSources);
    }

    public override Transition Tick(DPong game, float dt) {
      var result = Transition.None();

      while (_menu.Events.Count > 0) {
        var transition = HandleEvent(game, _menu.Events.Dequeue());
        if (transition.HasValue)
          result = transition.Value;
      }

      return result;
    }

    private Transition? HandleEvent(DPong game, in NetworkMenuEvent evt) {
      switch (evt.Type) {
        case NetworkMenuEventType.Back:
          return Transition.Pop();

        case NetworkMenuEventType.Play:
          if (TryStartSession(out var session))
            return Transition.Push(new WaitingNetworkScreen(session, _save.Input));

          ShowError(game.Ui, Tr._("Failed to connect to the server"));
          return null;

        case NetworkMenuEventType.NickChanged:
          var validatedNick = PlayerDataValidator.ValidateNickName(evt.GetNickName());
          _menu.SetPlayerName(validatedNick);
          _save.Name = validatedNick;
          return null;

        case NetworkMenuEventType.InputSrcChanged:
          var index = evt.GetInputSourceIndex();
          if (index >= 0 && index < game.InputSources.Descriptors.Count)
            _save.Input = game.InputSources.Descriptors[index];
          return null;

        case NetworkMenuEventType.ServerAddressChanged:
          var validated = evt.GetServerAddress().Trim();
          _menu.SetServerAddress(validated);
          _save.Host = validated;
          return null;

        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public override void Pause(DPong game) {
      _menu.Hide();
    }

    public override void Resume(DPong game) {
      UpdateMenu(game.InputSources);
      _menu.Show();
    }

    public override void Finish(DPong game) {
      game.Save.WriteCaches();
      game.Save.ReturnState(nameof(NetworkMenuScreen), _save);

      UnityEngine.Object.Destroy(_menu.gameObject);
      _menu = null;
    }

    private void UpdateMenu(InputSourceProvider inputSources) {
      _menu.SetPlayerName(_save.Name);
      _menu.SetServerAddress(_save.Host);

      inputSources.Refresh();
      _menu.SetInputSources(inputSources.Names, inputSources.Descriptors.IndexOf(_save.Input));
    }

    private bool TryStartSession(out ClientSession session) {
      var cfg = new DPongClientConfig(_save.Host, _save.Name);
      try {
        session = new ClientSession(cfg, null);
      }
      catch (Exception) {
        session = null;
        return false;
      }

      return true;
    }

    private void ShowError(UiSystem ui, string message) {
      var errMsg = ui.CreateErrorBox(false, message);
      errMsg.OnHideFinish += errMsg.Destroy;
      errMsg.Show();
    }
  }
}