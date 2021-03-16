using System.Collections.Generic;

namespace DPong.Localization.Editor {
  public readonly struct LocalizationEntry {
    public readonly List<string> Sources;
    public readonly string MsgId;
    public readonly string Context;

    public LocalizationEntry(string source, string msgId, string context) {
      Sources = new List<string> {source};
      MsgId = msgId;
      Context = context;
    }
  }
}