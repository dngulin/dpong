using NGIS.Session.Client;

namespace DPong.Level {
  public class DPongClientConfig : ClientConfig {
    private const int ProtocolVersion = 0;
    private const int DefaultPort = 5081;
    public DPongClientConfig(string host, string playerName) :
      base("dPONG", ProtocolVersion, 2, host, DefaultPort, playerName) { }
  }
}