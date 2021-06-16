using System;
using System.Collections.Generic;
using UnityEngine;

namespace DPong.StateTracker {
  public class GameStateTracker<TGame> {
    private readonly TGame _game;
    private readonly Stack<GameState<TGame>> _gameStates = new Stack<GameState<TGame>>();

    public GameStateTracker(TGame game, GameState<TGame> initialGameState) {
      _game = game;

      _gameStates.Push(initialGameState);
      initialGameState.Start(game);
    }

    public bool Tick(float dt) {
      if (_gameStates.Count == 0)
        return true;

      var transition = _gameStates.Peek().Tick(_game, dt);

      switch (transition.Type) {
        case TransitionType.None:
          break;

        case TransitionType.Push:
          _gameStates.Peek().Pause(_game);
          StartNewGameState(transition.GameState);
          break;

        case TransitionType.Pop:
          _gameStates.Pop().Finish(_game);
          if (_gameStates.Count > 0)
            _gameStates.Peek().Resume(_game);
          break;

        case TransitionType.Replace:
          _gameStates.Pop().Finish(_game);
          StartNewGameState(transition.GameState);
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }

      return _gameStates.Count == 0;
    }

    private void StartNewGameState(GameState<TGame> gameState) {
      Debug.Assert(gameState != null, "Try to start null game state");
      Debug.Assert(!_gameStates.Contains(gameState), "Try to add same game state twice");

      _gameStates.Push(gameState);
      gameState.Start(_game);
    }
  }
}