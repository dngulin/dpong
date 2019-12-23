using System;
using UnityEngine;
using ILogger = NGIS.Logging.ILogger;

namespace DPong.Level.Debugging {
  public class NgisUnityLogger : ILogger {
    public void Info(string msg) => Debug.Log($"SESSION: {msg}");

    public void Warning(string msg) => Debug.LogWarning($"SESSION: {msg}");

    public void Error(string msg) => Debug.LogError($"SESSION: {msg}");

    public void Exception(Exception e) => Debug.LogException(e);
  }
}