using DPong.Localization;
using NGIS.Message.Server;
using NGIS.Session.Client;

namespace DPong.Meta.Screens.NetworkMenu {
  public static class MessageConverter {
    public static string GetErrorMessage(SessionError error, ServerErrorId? serverErrorId = null) {
      switch (error) {
        case SessionError.InternalError: return Tr._("Internal error occured");
        case SessionError.ConnectionError: return Tr._("Connection lost");
        case SessionError.ProtocolError: return Tr._("Client protocol error");
        case SessionError.ServerError:
          return GetServerErrorMessage(serverErrorId);
        default: return Tr._("Unknown error");
      }
    }

    private static string GetServerErrorMessage(ServerErrorId? serverErrorId) {
      switch (serverErrorId) {
        case ServerErrorId.InternalError: return Tr._("Internal server error");
        case ServerErrorId.ProtocolError: return Tr._("Server protocol error");
        case ServerErrorId.Incompatible: return Tr._("Incompatible client version");
        case ServerErrorId.ServerIsBusy: return Tr._("Nickname is busy");
        case ServerErrorId.NickIsBusy: return Tr._("Nickname is busy");
        case ServerErrorId.ConnectionError: return Tr._("Server connection error");
        default: return Tr._("Unknown server error");
      }
    }
  }
}