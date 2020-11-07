using NGIS.Session.Client;

namespace DPong.Level {
  public class DPongClientConfig : ClientConfig {
    private const int ProtocolVersion = 0;
    public DPongClientConfig(string host, int port, string playerName) :
      base("dPONG", ProtocolVersion, 2, host, port, playerName) { }
  }
}