using DPong.Common;
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

    protected override void UpdateImpl(DynamicLevelState prev, DynamicLevelState curr, float factor) {
      SetScores(factor < 0.5f ? prev : curr);
    }

    private void SetScores(DynamicLevelState state) {
      _leftScore.text = state.LeftHp.ToString();
      _rightScore.text = state.RightHp.ToString();
    }

    protected override void DrawGizmos(DynamicLevelState dynState) {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireCube(Vector3.zero, StState.BoardSize.ToVector2());
    }
  }
}