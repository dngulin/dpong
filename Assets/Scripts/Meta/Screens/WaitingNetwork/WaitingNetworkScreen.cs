using System;
using DPong.InputSource.Data;
using DPong.Meta.Screens.NetworkGame;
using DPong.StateTracker;
using DPong.UI;
using NGIS.Session.Client;

namespace DPong.Meta.Screens.WaitingNetwork {
  public class WaitingNetworkScreen : GameState<DPong> {
    private ClientSession _session;
    private readonly InputSourceDescriptor _inputSource;

    private ConnectionDialog _connectingDlg;
    private bool _pendingExit;

    public WaitingNetworkScreen(ClientSession session, InputSourceDescriptor inputSource) {
      _session = session;
      _inputSource = inputSource;
    }

    public override void Start(DPong game) {
      var splash = game.Assets.LoadFromPrefab<ConnectionDialog>("Assets/Content/Meta/Prefabs/ConnectionDialog.prefab");
      _connectingDlg = game.Ui.InstantiateWindow(WindowType.Dialog, splash, false);
      _connectingDlg.OnCancelClicked += () => _pendingExit = true;
      _connectingDlg.OnHideFinish += () => {
        _connectingDlg.Destroy();
        _connectingDlg = null;
      };

      _connectingDlg.SetJoinedState(false);
      _connectingDlg.Show();
    }

    public override Transition Tick(DPong game, float dt) {
      if (_pendingExit)
        return Transition.Pop();

      var result = _session.Process();
      switch (result.Type) {
        case ProcessingResult.ResultType.None:
          return Transition.None();

        case ProcessingResult.ResultType.Error:
          _connectingDlg.Hide();
          ShowError(game.Ui, MessageConverter.GetErrorMessage(result.SessionError, result.ServerErrorId));
          return Transition.Pop();

        case ProcessingResult.ResultType.Joined:
          _connectingDlg.SetJoinedState(true);
          return Transition.None();

        case ProcessingResult.ResultType.Started:
          _connectingDlg.Hide();

          var session = _session;
          _session = null;

          return Transition.Replace(new NetworkGameScreen(game, session, result.StartMessage, _inputSource));

        default:
          throw new ArgumentOutOfRangeException($"Impossible session prcoessing result {result.Type}");
      }
    }

    private void ShowError(UiSystem ui, string message) {
      var errMsg = ui.CreateErrorBox(false, message);
      errMsg.OnHideFinish += errMsg.Destroy;
      errMsg.Show();
    }

    public override void Pause(DPong game) => throw new NotSupportedException();
    public override void Resume(DPong game) => throw new NotSupportedException();

    public override void Finish(DPong game) => _session?.Dispose();
  }
}