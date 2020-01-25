using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DPong.Core.Save {
  public interface IMigrations {
    void PreprocessSave(Dictionary<string, JObject> save, bool isFirstRun);
  }
}