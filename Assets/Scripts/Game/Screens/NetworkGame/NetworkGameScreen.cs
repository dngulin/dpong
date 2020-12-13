using System;
using DPong.Game.Navigation;
using DPong.Game.Validation;
using DPong.InputSource;
using DPong.InputSource.Extensions;
using DPong.Level;
using DPong.Localization;
using DPong.Save;
using DPong.UI;
using NGIS.Message.Server;
using NGIS.Session.Client;
using UnityEngine;

namespace DPong.Game.Screens.NetworkGame {
  public class NetworkGameScreen : INavigationPoint, INetworkGameMenuListener, ILevelExitListener, IDisposable {
    private readonly Navigator _navigator;
    private readonly UISystem _uiSystem;
    private readonly InputSourceProvider _inputSources;
    private readonly SaveSystem _saveSystem;

    private readonly NetworkGameSave _save;

    private NetworkGameMenu _menu;
    private ConnectionDialog _connectingDlg;

    private bool _isConnecting;

    private ClientSession _session;
    private NetworkLevel _level;

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

      if (_connectingDlg != null)
        UnityEngine.Object.Destroy(_connectingDlg.gameObject);

      _disposed = true;
    }

    void INavigationPoint.Enter() {
      _menu = _uiSystem.Instantiate(Resources.Load<NetworkGameMenu>("NetworkGameMenu"), UILayer.Background, true);
      _menu.Init(this);
      UpdateMenu();

      var splash = Resources.Load<ConnectionDialog>("ConnectionDialog");
      _connectingDlg = _uiSystem.InstantiateWindow(WindowType.Dialog, splash, false);
      _connectingDlg.OnCancelClicked += StopConnecting;
    }

    void INavigationPoint.Suspend() => HideMenu();

    void INavigationPoint.Resume() => ShowMenu();

    void INavigationPoint.Exit() {
      _saveSystem.WriteSaveToFile();

      UnityEngine.Object.Destroy(_menu.gameObject);
      _menu = null;
    }

    void INavigationPoint.Tick(float dt) => Tick();

    private void ShowMenu() {
      UpdateMenu();
      _menu.Show();
    }

    private void UpdateMenu() {
      _menu.SetPlayerName(_save.Name);
      _menu.SetServerAddress(_save.Host);

      _inputSources.Refresh();
      _menu.SetInputSources(_inputSources.Names, _inputSources.Descriptors.IndexOf(_save.Input));
    }

    private void HideMenu() => _menu.Hide();

    void ILevelExitListener.Exit() {
      _level?.Dispose();
      _session?.Dispose();

      _level = null;
      _session = null;

      ShowMenu();
    }

    void INetworkGameMenuListener.PlayClicked() {
      var cfg = new DPongClientConfig(_save.Host, _save.Name);
      try {
        _session = new ClientSession(cfg, null);
      }
      catch (Exception) {
        ShowError(Tr._("Failed to connect to the server"));
        // TODO: add log?
        return;
      }

      _isConnecting = true;
      _connectingDlg.SetJoinedState(false);
      _connectingDlg.Show();
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
          _connectingDlg.SetJoinedState(true);
          break;
        case ProcessingResult.ResultType.Started:
          HandleSessionStarted(result.StartMessage);
          break;
        case ProcessingResult.ResultType.Active:
          HandleActiveSession();
          break;
        case ProcessingResult.ResultType.Finished:
          _level.ExitWithFinish();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void HandleSessionStarted(ServerMsgStart msgStart) {
      _isConnecting = false;
      _connectingDlg.Hide();
      HideMenu();

      var inputSource = _inputSources.CreateSource(_save.Input);
      _level = new NetworkLevel(inputSource, _uiSystem, this, msgStart);
    }

    private void HandleActiveSession() {
      if (!_level.HandleReceivedInputs(_session.ReceivedInputs)) {
        HandleSessionError(SessionError.ProtocolError);
        return;
      }

      var (inputs, optMsgFinish) = _level.Tick();
      var optError = _session.SendMessages(inputs, optMsgFinish);
      if (optError.HasValue)
        HandleSessionError(optError.Value);
    }

    private void HandleSessionError(SessionError error, ServerErrorId? serverErrorId = null) {
      var errorMessage = MessageConverter.GetErrorMessage(error, serverErrorId);
      if (_isConnecting) {
        _connectingDlg.Hide();
        StopConnecting();
        ShowError(errorMessage);
      }
      else {
        _level.ExitWithError(errorMessage);
      }
    }

    private void ShowError(string message) {
      var errMsg = _uiSystem.CreateErrorBox(false, message);
      errMsg.OnHideFinish += errMsg.Destroy;
      errMsg.Show();
    }

    private void StopConnecting() {
      if (!_isConnecting)
        return;

      _isConnecting = false;

      _session.Dispose();
      _session = null;
    }
  }
}