using UnityEngine;
using UnityEngine.UI;

namespace DPong.Level.Debugging.UI {
  public class DbgConnectMenu : DebugUIBase {
    [SerializeField] private InputField _host;
    [SerializeField] private InputField _name;

    public string PlayerName {
      get => _name.text;
      set => _name.text = value;
    }

    public string HostName {
      get => _host.text;
      set => _host.text = value;
    }
  }


}