using System;
using System.Collections.Generic;
using DPong.Game.Navigation;
using DPong.InputSource;
using DPong.Level;
using DPong.Save;
using DPong.UI;
using NGIS.Message.Client;
using NGIS.Message.Server;
using NGIS.Session.Client;
using UnityEngine;

namespace DPong.Game.Screens.NetworkGame {
  // TODO: Too many interfaces. Aggregate session and level in one object (ILevelExitListener, IClientSessionWorker)
  public class NetworkGameScreen : INavigationPoint, INetworkGameMenuListener, ILevelExitListener, IClientSessionWorker, ITickable, IDisposable {
    private readonly Navigator _navigator;
    private readonly SaveSystem _saveSystem;
    private readonly InputSourceProvider _inputSources;

    private readonly UISystem _uiSystem;

    private ClientSession _networkSession;
    private NetworkLevelController _levelController;

    private NetworkGameMenu _menu;
    private readonly NetworkGameSave _save;

    public NetworkGameScreen(SaveSystem save, InputSourceProvider inputSources, UISystem ui, Navigator navigator) {
      _saveSystem = save;
      _inputSources = inputSources;
      _uiSystem = ui;
      _navigator = navigator;

      _save = _saveSystem.TakeState(nameof(NetworkGameScreen), new NetworkGameSave());
    }

    public void Dispose() {
      _levelController?.Dispose();
      _networkSession?.Dispose();
    }

    void INavigationPoint.Enter() {
      _menu = _uiSystem.Instantiate(Resources.Load<NetworkGameMenu>("NetworkGameMenu"), UILayer.Background, true);
      // Set listener: _menu.Init(this);
      // Set menu values from save
    }

    void INavigationPoint.Suspend() => _menu.Hide();

    void INavigationPoint.Resume() {
      _menu.Show();
      // Set menu values from save
    }

    void INavigationPoint.Exit() {
      _saveSystem.WriteSaveToFile();

      UnityEngine.Object.Destroy(_menu.gameObject);
      _menu = null;
    }

    void ITickable.FixedTick() {}

    void ITickable.DynamicTick(float dt) {
      _networkSession?.Process();
    }

    void ILevelExitListener.Exit() {
      _levelController?.Dispose();
      _networkSession?.Dispose();
      ((INavigationPoint) this).Resume();
    }

    void IClientSessionWorker.JoinedToSession() {
      // update connecting menu msg
    }

    void IClientSessionWorker.SessionStarted(ServerMsgStart msgStart) {
      var inputSource = _inputSources.CreateSource(_save.Input);
      _levelController = new NetworkLevelController(inputSource, msgStart);
    }

    void IClientSessionWorker.InputReceived(ServerMsgInput msgInput) {
      _levelController.InputReceived(msgInput);
    }

    (Queue<ClientMsgInputs>, ClientMsgFinished?) IClientSessionWorker.Process() => _levelController.Process();

    void IClientSessionWorker.SessionFinished(ServerMsgFinish msgFinish) {
      // show result in level
    }

    void IClientSessionWorker.SessionClosedWithError(ClientSessionError errorId, ServerErrorId? serverErrorId) {
      // show error in connection screen or in level
    }
  }
}