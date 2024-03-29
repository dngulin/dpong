using DPong.Level.Data;
using UnityEngine;

namespace DPong.Level.View {
  public class PaddleView: MonoBehaviour {
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _leftSprite;
    [SerializeField] private Sprite _rightSprite;

    public PaddleView Configured(Side side) {
      _spriteRenderer.sprite = side == Side.Left ? _leftSprite : _rightSprite;
      return this;
    }

    public void SetPosition(in Vector2 pos) => transform.localPosition = pos;
  }
}