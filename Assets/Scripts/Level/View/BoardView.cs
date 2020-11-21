using DPong.Level.Data;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Level.View {
  public class BoardView: MonoBehaviour {
    private const char HpChar = '@';

    [SerializeField] private Text _leftName;
    [SerializeField] private Text _rightName;

    [SerializeField] private Text _leftHp;
    [SerializeField] private Text _rightHp;

    private int _leftHpValue;
    private int _rightHpValue;

    public BoardView Configured(PlayerInfo l, PlayerInfo r) {
      _leftName.text = GetPrefixedName(l);
      _rightName.text = GetPrefixedName(r);
      return this;
    }

    private static string GetPrefixedName(PlayerInfo info) => $"[{info.Type}]\n{info.Name}";

    public void SetHitPoints(int left, int right) {
      if (_leftHpValue != left) {
        _leftHpValue = left;
        _leftHp.text = new string(HpChar, _leftHpValue);
      }

      if (_rightHpValue != right) {
        _rightHpValue = right;
        _rightHp.text = new string(HpChar, _rightHpValue);
      }
    }
  }
}