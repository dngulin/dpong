using System;
using DPong.InputSource.Data;
using DPong.Level;
using DPong.Meta.Screens.NetworkMenu;
using DPong.StateTracker;
using NGIS.Message.Server;
using NGIS.Session.Client;

namespace DPong.Meta.Screens.NetworkGame {
  public class NetworkGameScreen : GameState<DPong>, ILevelExitListener {
    private readonly ClientSession _session;
    private readonly NetworkLevel _level;

    private bool _pendingExit;

    public NetworkGameScreen(DPong game, ClientSession session, ServerMsgStart startMsg, in InputSourceDescriptor inputSrc) {
      _session = session;

      var inputSource = game.InputSources.CreateSource(inputSrc);
      var levelViewFactory = new LevelViewFactory(game.Assets, game.Ui);

      _level = new NetworkLevel(inputSource, levelViewFactory, this, startMsg);
    }

    void ILevelExitListener.Exit() => _pendingExit = true;

    public override void Start(DPong game) {
    }

    public override Transition Tick(DPong game, float dt) {
      if (_pendingExit)
        return Transition.Pop();

      var result = _session.Process();
      switch (result.Type) {
        case ProcessingResult.ResultType.None:
          break;

        case ProcessingResult.ResultType.Error:
          _level.ExitWithError(MessageConverter.GetErrorMessage(result.SessionError, result.ServerErrorId));
          break;

        case ProcessingResult.ResultType.Active:
          TickLevel();
          break;

        case ProcessingResult.ResultType.Finished:
          _level.ExitWithFinish();
          break;

        default:
          throw new ArgumentOutOfRangeException($"Impossible session prcoessing result {result.Type}");
      }

      return Transition.None();
    }

    private void TickLevel() {
      if (!_level.HandleReceivedInputs(_session.ReceivedInputs)) {
        _level.ExitWithError(MessageConverter.GetErrorMessage(SessionError.ProtocolError));
        return;
      }

      var (inputs, optMsgFinish) = _level.Tick();
      var optError = _session.SendMessages(inputs, optMsgFinish);
      if (optError.HasValue)
        _level.ExitWithError(MessageConverter.GetErrorMessage(optError.Value));
    }

    public override void Pause(DPong game) => throw new NotSupportedException();
    public override void Resume(DPong game) => throw new NotSupportedException();

    public override void Finish(DPong game) {
      _level.Dispose();
      _session.Dispose();
    }
  }
}