using UnityEngine;
using UnityEngine.UI;

namespace DPong.Localization {

  [RequireComponent(typeof(Text))]
  public class LocalizedTextLoader : MonoBehaviour {
    [SerializeField] private string _context;

    public string Context => _context;

    public void Awake() {
      var textComponent = GetComponent<Text>();
      if (textComponent == null)
        return;

      textComponent.text = string.IsNullOrEmpty(_context) ?
        T._(textComponent.text) :
        T._p(_context, textComponent.text);
    }
  }
}