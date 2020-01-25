using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DPong.Core.Save {
  public class SaveSystem {
    private readonly UTF8Encoding _utf8;
    private readonly string _saveFilePath;

    private readonly Dictionary<string, object> _memoryStates = new Dictionary<string, object>();
    private readonly Dictionary<string, JObject> _serializedStates = new Dictionary<string, JObject>();

    private readonly JsonSerializer _serializer;

    public SaveSystem(string fileName, IMigrations migrations = null) {
      _saveFilePath = Path.Combine(Application.persistentDataPath, fileName);

      _utf8 = new UTF8Encoding(false);
      _serializer = new JsonSerializer {
        Formatting = Formatting.Indented,
        ObjectCreationHandling = ObjectCreationHandling.Replace
      };

      var isFirstRun = true;

      if (File.Exists(_saveFilePath)) {
        isFirstRun = false;

        using (var textReader = new StreamReader(File.OpenRead(_saveFilePath), _utf8))
        using (var jsonReader = new JsonTextReader(textReader)) {
          _serializedStates = _serializer.Deserialize<Dictionary<string, JObject>>(jsonReader);
        }
      }

      migrations?.PreprocessSave(_serializedStates, isFirstRun);
    }

    public T TakeState<T>(string name, T defaultValue) where T : class {
      if (_memoryStates.ContainsKey(name))
        throw new SaveSystemException("State tate already taken");

      T state;
      if (_serializedStates.TryGetValue(name, out var serialized)) {
        state = serialized.ToObject<T>(_serializer) ?? defaultValue;
      }
      else {
        state = defaultValue;
      }

      _memoryStates.Add(name, state);
      return state;
    }

    public void ReturnState(string name, object state) {
      if (!_memoryStates.TryGetValue(name, out var takenState))
        throw new SaveSystemException($"State with name '{name}' is not taken");

      if (!ReferenceEquals(state, takenState))
        throw new SaveSystemException("Try to return non-taken state");

      _memoryStates.Remove(name);
      _serializedStates[name] = JObject.FromObject(state, _serializer);
    }

    public void WriteSaveToFile() {
      using (var textWriter = new StreamWriter(File.Create(_saveFilePath), _utf8))
      using (var jsonWriter = new JsonTextWriter(textWriter)) {
        _serializer.Serialize(jsonWriter, _serializedStates);
      }
    }
  }
}