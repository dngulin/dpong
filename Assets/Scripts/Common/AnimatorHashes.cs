using System;
using System.Collections.Generic;
using UnityEngine;

namespace DPong.Common {
  public class AnimatorHashes<TState, TParam> where TState : unmanaged where TParam : unmanaged {
    public readonly Dictionary<TState, int> States;
    public readonly Dictionary<TParam, int> Params;

    public readonly Dictionary<int, TState> HashToStateMap = new Dictionary<int, TState>();

    public AnimatorHashes() {
      States = new Dictionary<TState, int>(new EnumComparer<TState>());
      Params = new Dictionary<TParam, int>(new EnumComparer<TParam>());

      foreach (var state in (TState[]) Enum.GetValues(typeof(TState))) {
        var stateHash = Animator.StringToHash(state.ToString());

        States.Add(state, stateHash);
        HashToStateMap.Add(stateHash, state);
      }

      foreach (var param in (TParam[]) Enum.GetValues(typeof(TParam))) {
        Params.Add(param, Animator.StringToHash(param.ToString()));
      }
    }

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