using System;
using NGIS.Logging;
using UnityEngine;

namespace DPong.Level.Debugging {
  public class NgisUnityLogger : IClientSessionLogger {
    public void Connecting(string host, int port) => Debug.Log($"Connecting to {host}:{port}");

    public void Joining(string playerName, string game, ushort version) {
      Debug.Log($"Joining as {playerName} ({game}:{version})");
    }

    public void Joined() => Debug.Log("Joined!");
    public void GameStarted() => Debug.Log("Game started!");
    public void GameFinished() => Debug.Log("Game finished!");
    public void ConnectionLost() => Debug.LogError("Connection lost!");
    public void SessionClosed() => Debug.Log("Session closed");

    public void FailedToProcessSession(Exception exception) {
      Debug.LogError("Failed to process session!");
      Debug.LogError(exception);
    }

    public void FailedToSendMessages(Exception exception) {
      Debug.LogError("Failed to send messages!");
      Debug.LogError(exception);
    }
  }
}