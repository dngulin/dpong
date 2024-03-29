using UnityEngine;
using UnityEngine.UI;

namespace DPong.Localization {

  [RequireComponent(typeof(Text))]
  public class TranslationLoader : MonoBehaviour {
    [SerializeField] private string _context;

    public string Context => _context;

    public void Awake() {
      var textComponent = GetComponent<Text>();
      if (textComponent == null)
        return;

      textComponent.text = string.IsNullOrEmpty(_context) ?
        Tr._(textComponent.text) :
        Tr._p(_context, textComponent.text);
    }
  }
}