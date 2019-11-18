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

    protected override void InitImpl(LevelState state) {
      _leftName.text = GetPrefixedName(Settings.PlayerLeft);
      _rightName.text = GetPrefixedName(Settings.PlayerRight);
    }

    private static string GetPrefixedName(PlayerInfo info) => info.IsBot ? $"[AI] {info.Name}" : info.Name;

    protected override void UpdateImpl(LevelState prev, LevelState curr, float factor) {
      SetScores(factor < 0.5f ? prev : curr);
    }

    private void SetScores(LevelState state) {
      _leftScore.text = state.HitPoints.Left.ToString();
      _rightScore.text = state.HitPoints.Right.ToString();
    }

    protected override void DrawGizmosImpl(LevelState state) {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireCube(Vector3.zero, Settings.Board.Size.ToVector2());
    }
  }
}