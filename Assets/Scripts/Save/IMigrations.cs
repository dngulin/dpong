using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DPong.Save {
  public interface IMigrations {
    void PreprocessSave(Dictionary<string, JObject> save, bool isFirstRun);
  }
}