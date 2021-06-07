using DPong.Level.Data;
using UnityEngine;

namespace DPong.Level.View {
  public class StateGizmoDrawer : MonoBehaviour {
    public bool DrawInterpolationSources;

    private LevelSettings _settings;

    private LevelViewState _left;
    private LevelViewState _right;
    private LevelViewState _interpolated;

    public void Init(LevelSettings settings) {
      _settings = settings;
      DrawInterpolationSources = true;
    }

    public void SetInterpolationSources(in LevelViewState l, in LevelViewState r) {
      _left = l;
      _right = r;
    }

    public void SetInterpolationResult(in LevelViewState i) => _interpolated = i;

    private void OnDrawGizmos() {
      DrawStaticState(Color.white);

      if (DrawInterpolationSources) {
        DrawDynamicState(_left, Color.green);
        DrawDynamicState(_right, Color.red);
      }

      DrawDynamicState(_interpolated, Color.yellow);
    }

    private void DrawStaticState(Color color) {
      Gizmos.color = color;

      var boardSize = ToVector(_settings.Board.Size);
      Gizmos.DrawWireCube(Vector3.zero, ToVector(_settings.Board.Size));

      var gatesSize = ToVector(_settings.Board.GateSize);
      var shift = new Vector2(boardSize.x + gatesSize.x, 0) / 2;
      Gizmos.DrawWireCube(shift, gatesSize);
      Gizmos.DrawWireCube(-shift, gatesSize);
    }

    private void DrawDynamicState(in LevelViewState state, Color color) {
      Gizmos.color = color;

      var ballRadius = _settings.Ball.Radius.ToFloat();
      Gizmos.DrawWireSphere(state.Ball.Position, ballRadius);

      Gizmos.DrawWireCube(state.Blockers[0].Position, ToVector(_settings.Blocker.Size));
      Gizmos.DrawWireCube(state.Blockers[1].Position, ToVector(_settings.Blocker.Size));
    }

    private static Vector2 ToVector(in FxRectSize rect) {
      return new Vector2(rect.Width.ToFloat(), rect.Height.ToFloat());
    }
  }
}