using System;
using DPong.Common;
using DPong.Level.Data;
using DPong.Level.State;
using PGM.ScaledNum;
using UnityEngine;

namespace DPong.Level.View {
  public class BlockerView: StateViewer {
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _leftSprite;
    [SerializeField] private Sprite _rightSprite;

    private Side _trackingSide;

    public BlockerView ConfiguredForSide(Side side) {
      _spriteRenderer.sprite = side == Side.Left ? _leftSprite : _rightSprite;
      _trackingSide = side;

      return this;
    }

    protected override void UpdateImpl(LevelState prev, LevelState curr, float factor) {
      switch (_trackingSide) {
        case Side.Left:
          UpdateBlockerPosition(prev.Blockers.LeftPosition, curr.Blockers.LeftPosition, factor);
          break;

        case Side.Right:
          UpdateBlockerPosition(prev.Blockers.RightPosition, curr.Blockers.RightPosition, factor);
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void UpdateBlockerPosition(in SnVector2 prev, in SnVector2 curr, float factor) {
      transform.localPosition = Vector3.Lerp(prev.ToVector2(), curr.ToVector2(), factor);
    }

    protected override void DrawGizmos(LevelState state) {
      var position = _trackingSide == Side.Left ? state.Blockers.LeftPosition : state.Blockers.RightPosition;

      Gizmos.color = Color.green;
      Gizmos.DrawWireCube(position.ToVector2(), StState.BlockerSize.ToVector2());
    }
  }
}