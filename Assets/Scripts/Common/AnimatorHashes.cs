using System;
using System.Collections.Generic;
using UnityEngine;

namespace DPong.Common {
  public class AnimatorHashes<TState, TParam> where TState : unmanaged where TParam : unmanaged {
    private readonly Dictionary<TState, int> _states;
    private readonly Dictionary<TParam, int> _params;

    private readonly Dictionary<int, TState> _hashToStateMap = new Dictionary<int, TState>();

    public AnimatorHashes() {
      _states = new Dictionary<TState, int>(new EnumComparer<TState>());
      _params = new Dictionary<TParam, int>(new EnumComparer<TParam>());

      foreach (var state in (TState[]) Enum.GetValues(typeof(TState))) {
        var stateHash = Animator.StringToHash(state.ToString());

        _states.Add(state, stateHash);
        _hashToStateMap.Add(stateHash, state);
      }

      foreach (var param in (TParam[]) Enum.GetValues(typeof(TParam))) {
        _params.Add(param, Animator.StringToHash(param.ToString()));
      }
    }

    public int GetStateHash(TState state) => _states[state];
    public int GetParameterHash(TParam state) => _params[state];

    public bool TryGetStateByHash(int hash, out TState state) => _hashToStateMap.TryGetValue(hash, out state);

    private class EnumComparer<TEnum> : IEqualityComparer<TEnum> where TEnum : unmanaged {
      public bool Equals(TEnum x, TEnum y) => AsInt(x) == AsInt(y);
      public int GetHashCode(TEnum obj) => AsInt(obj);

      private static unsafe int AsInt(TEnum value) {
        switch (sizeof(TEnum)) {
          case sizeof(int):
            return *(int*) &value;

          case sizeof(short):
            return *(short*) &value;

          case sizeof(byte):
            return *(byte*) &value;

          case sizeof(long):
            return (int) *(long*) &value;
        }

        throw new InvalidOperationException($"Failed to cast the enum {typeof(TEnum)} to int");
      }
    }
  }
}