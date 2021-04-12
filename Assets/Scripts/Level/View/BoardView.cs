using DPong.Level.Data;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Level.View {
  public class BoardView: MonoBehaviour {
    [SerializeField] private Text _leftName;
    [SerializeField] private Text _rightName;

    [SerializeField] private Text _score;

    private int _leftScoreValue;
    private int _rightScoreValue;

    public BoardView Configured(PlayerInfo l, PlayerInfo r) {
      _leftName.text = GetPrefixedName(l);
      _rightName.text = GetPrefixedName(r);
      return this;
    }

    private static string GetPrefixedName(PlayerInfo info) => $"[{info.Type}]\n{info.Name}";

    public void SetScore(int left, int right) {
      if (_leftScoreValue == left && _rightScoreValue == right)
        return;

      _leftScoreValue = left;
      _rightScoreValue = right;
      _score.text = $"{left} : {right}";
    }
  }
}