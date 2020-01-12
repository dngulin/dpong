using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DPong.Level.Debugging.UI {
  public class Popup : DebugUIBase {
    [SerializeField] private Text _message;

    public string Text {
      set => _message.text = value;
    }
  }
}