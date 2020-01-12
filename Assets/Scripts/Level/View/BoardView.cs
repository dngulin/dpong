using DPong.Common;
using DPong.Level.Data;
using DPong.Level.State;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Level.View {
  public class BoardView: StateViewer {
    private const char HpChar = '@';

    [SerializeField] private Text _leftName;
    [SerializeField] private Text _rightName;

    [SerializeField] private Text _leftHp;
    [SerializeField] private Text _rightHp;

    private int _leftHpValue = -1;
    private int _rightHpValue = -1;

    protected override void InitImpl(LevelState state) {
      _leftName.text = GetPrefixedName(Settings.PlayerLeft);
      _rightName.text = GetPrefixedName(Settings.PlayerRight);
      SetScores(state);
    }

    private static string GetPrefixedName(PlayerInfo info) => $"[{info.Type}] {info.Name}";

    protected override void UpdateImpl(LevelState prev, LevelState curr, float factor) {
      SetScores(factor < 0.5f ? prev : curr);
    }

    private void SetScores(LevelState state) {
      if (_leftHpValue != state.HitPoints.Left) {
        _leftHpValue = state.HitPoints.Left;
        _leftHp.text = new string(HpChar, _leftHpValue);
      }

      if (_rightHpValue != state.HitPoints.Right) {
        _rightHpValue = state.HitPoints.Right;
        _rightHp.text = new string(HpChar, _rightHpValue);
      }
    }

    protected override void DrawGizmosImpl(LevelState state) {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireCube(Vector3.zero, Settings.Board.Size.ToVector2());
    }
  }
}