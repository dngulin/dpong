using System;
using DPong.Game.Navigation;
using DPong.Game.Validation;
using DPong.InputSource;
using DPong.InputSource.Extensions;
using DPong.Level;
using DPong.Save;
using DPong.UI;
using NGIS.Message.Server;
using NGIS.Session.Client;
using UnityEngine;

namespace DPong.Game.Screens.NetworkGame {
  // TODO: Too many interfaces. Aggregate session and level in one object (ILevelExitListener, IClientSessionWorker)
  public class NetworkGameScreen : INavigationPoint, INetworkGameMenuListener, ILevelExitListener, ITickable, IDisposable {
    private readonly Navigator _navigator;
    private readonly SaveSystem _saveSystem;
    private readonly InputSourceProvider _inputSources;

    private readonly UISystem _uiSystem;

    private ClientSession _session;
    private NetworkLevelController _level;

    private NetworkGameMenu _menu;
    private readonly NetworkGameSave _save;

    private bool _disposed;

    public NetworkGameScreen(SaveSystem save, InputSourceProvider inputSources, UISystem ui, Navigator navigator) {
      _saveSystem = save;
      _inputSources = inputSources;
      _uiSystem = ui;
      _navigator = navigator;

      _save = _saveSystem.TakeState(nameof(NetworkGameScreen), new NetworkGameSave());
    }

    public void Dispose() {
      if (_disposed)
        return;

      _level?.Dispose();
      _session?.Dispose();

      _saveSystem.ReturnState(nameof(NetworkGameScreen), _save);

      if (_menu != null)
        UnityEngine.Object.Destroy(_menu.gameObject);

      _disposed = true;
    }

    void ITickable.FixedTick() {}
    void ITickable.DynamicTick(float dt) => Tick();

    void INavigationPoint.Enter() {
      _menu = _uiSystem.Instantiate(Resources.Load<NetworkGameMenu>("NetworkGameMenu"), UILayer.Background, true);
      _menu.Init(this);
      UpdateMenu();
    }

    void INavigationPoint.Suspend() => _menu.Hide();

    void INavigationPoint.Resume() {
      UpdateMenu();
      _menu.Show();
    }

    private void UpdateMenu() {
      _menu.SetPlayerName(_save.Name);
      _menu.SetServerAddress(_save.Host);

      _inputSources.Refresh();
      _menu.SetInputSources(_inputSources.Names, _inputSources.Descriptors.IndexOf(_save.Input));
    }

    void INavigationPoint.Exit() {
      _saveSystem.WriteSaveToFile();

      UnityEngine.Object.Destroy(_menu.gameObject);
      _menu = null;
    }

    void ILevelExitListener.Exit() {
      _level?.Dispose();
      _session?.Dispose();
      ((INavigationPoint) this).Resume();
    }

    void INetworkGameMenuListener.PlayClicked() {
      var cfg = new DPongClientConfig(_save.Host, _save.Name);
      try {
        _session = new ClientSession(cfg, null);
      }
      catch (Exception e) {
        var errMsg = _uiSystem.CreateErrorBox(false, e.Message);
        errMsg.OnHideFinish += errMsg.Destroy;
        errMsg.Show();
        return;
      }

      // TODO: Show connection UI
    }

    void INetworkGameMenuListener.BackClicked() => _navigator.Exit(this);

    void INetworkGameMenuListener.PlayerNameChanged(string name) {
      var validated = PlayerDataValidator.ValidateNickName(name);
      _menu.SetPlayerName(validated);
      _save.Name = validated;
    }

    void INetworkGameMenuListener.InputSourceChanged(int index) {
      if (index < 0 || index >= _inputSources.Descriptors.Count)
        return;

      _save.Input = _inputSources.Descriptors[index];
    }

    void INetworkGameMenuListener.ServerAddressChanged(string address) {
      var validated = address.Trim();
      _menu.SetServerAddress(validated);
      _save.Host = validated;
    }

    private void Tick() {
      if (_session == null)
        return;

      var result = _session.Process();
      switch (result.Type) {
        case ProcessingResult.ResultType.None:
          break;
        case ProcessingResult.ResultType.Error:
          HandleSessionError(result.SessionError, result.ServerErrorId);
          break;
        case ProcessingResult.ResultType.Joined:
          // TODO: Update Connection UI
          break;
        case ProcessingResult.ResultType.Started:
          HandleSessionStarted(result.StartMessage);
          break;
        case ProcessingResult.ResultType.Active:
          HandleActiveSession();
          break;
        case ProcessingResult.ResultType.Finished:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void HandleSessionError(SessionError error, ServerErrorId? serverErrorId = null) {

    }

    private void HandleSessionStarted(ServerMsgStart msgStart) {
      // TODO: Hide Connection UI
      var inputSource = _inputSources.CreateSource(_save.Input);
      _level = new NetworkLevelController(inputSource, msgStart);
    }

    private void HandleActiveSession() {
      if (!_level.HandleReceivedInputs(_session.ReceivedInputs)) {
        HandleSessionError(SessionError.ProtocolError);
        return;
      }

      var (inputs, optMsgFinish) = _level.Process();
      var optError = _session.SendMessages(inputs, optMsgFinish);
      if (optError.HasValue)
        HandleSessionError(optError.Value);
    }
  }
}