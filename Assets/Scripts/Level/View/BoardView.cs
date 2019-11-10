using DPong.Level.Data;
using DPong.Level.State;
using UnityEngine;

namespace DPong.Level.View {
  public class BoardView: StateViewer {
    [SerializeField] private TextMesh _leftName;
    [SerializeField] private TextMesh _rightName;

    [SerializeField] private TextMesh _leftScore;
    [SerializeField] private TextMesh _rightScore;

    public BoardView ConfiguredForPlayers(PlayerInfo left, PlayerInfo right) {
      _leftName.text = GetPrefixedName(left);
      _rightName.text = GetPrefixedName(right);
      return this;
    }

    private static string GetPrefixedName(PlayerInfo info) => info.IsBot ? $"[AI] {info.Name}" : info.Name;

    protected override void UpdateImpl(LevelState previous, LevelState current, float factor) {
      SetScores(factor < 0.5f ? previous : current);
    }

    private void SetScores(LevelState state) {
      _leftScore.text = state.LeftScore.ToString();
      _rightScore.text = state.RightScore.ToString();
    }

    protected override void DrawGizmos(LevelState state) {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireCube(Vector3.zero, new Vector3(30, 20, 0));
    }
  }
}