namespace DPong.StateTracker {
  public abstract class GameState<TGame> {
    public abstract void Start(TGame game);
    public abstract Transition Tick(TGame game, float dt);
    public abstract void Pause(TGame game);
    public abstract void Resume(TGame game);
    public abstract void Finish(TGame game);


    public readonly struct Transition {
      public readonly TransitionType Type;
      public readonly GameState<TGame> GameState;

      private Transition(TransitionType type, GameState<TGame> gameState) {
        Type = type;
        GameState = gameState;
      }

      public static Transition None() => new Transition(TransitionType.None, null);
      public static Transition Push(GameState<TGame> state) => new Transition(TransitionType.Push, state);
      public static Transition Pop() => new Transition(TransitionType.Pop, null);
      public static Transition Replace(GameState<TGame> state) => new Transition(TransitionType.Replace, state);
    }
  }
}