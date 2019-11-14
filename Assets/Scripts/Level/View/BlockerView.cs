using System;
using DPong.Common;
using DPong.Level.Data;
using DPong.Level.State;
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

    protected override void UpdateImpl(DynamicLevelState prev, DynamicLevelState curr, float factor) {
      switch (_trackingSide) {
        case Side.Left:
          UpdateBlockerPosition(prev.LeftBlocker, curr.LeftBlocker, factor);
          break;

        case Side.Right:
          UpdateBlockerPosition(prev.RightBlocker, curr.RightBlocker, factor);
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void UpdateBlockerPosition(ColliderState prev, ColliderState curr, float factor) {
      var prevPos = prev.Position.ToVector2();
      var currPos = curr.Position.ToVector2();
      transform.localPosition = Vector3.Lerp(prevPos, currPos, factor);
    }

    protected override void DrawGizmos(DynamicLevelState dynState) {
      var blocker = _trackingSide == Side.Left ? dynState.LeftBlocker : dynState.RightBlocker;

      Gizmos.color = Color.green;
      Gizmos.DrawWireCube(blocker.Position.ToVector2(), StState.BlockerSize.ToVector2());
    }
  }
}