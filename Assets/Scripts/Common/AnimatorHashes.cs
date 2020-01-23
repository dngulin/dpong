using System;
using System.Collections.Generic;
using UnityEngine;

namespace DPong.Common {
  public class AnimatorHashes<TState, TParam> where TState : unmanaged, Enum where TParam : unmanaged, Enum {
    public readonly Dictionary<TState, int> States;
    public readonly Dictionary<TParam, int> Params;

    public readonly Dictionary<int, TState> HashToStateMap = new Dictionary<int, TState>();

    public AnimatorHashes(Func<TState, int> stateAsInt, Func<TParam, int> paramAsInt) {
      States = new Dictionary<TState, int>(new EqComparer<TState>(stateAsInt));
      Params = new Dictionary<TParam, int>(new EqComparer<TParam>(paramAsInt));

      foreach (var state in (TState[]) Enum.GetValues(typeof(TState))) {
        var stateHash = Animator.StringToHash(state.ToString());

        States.Add(state, stateHash);
        HashToStateMap.Add(stateHash, state);
      }

      foreach (var param in (TParam[]) Enum.GetValues(typeof(TParam))) {
        Params.Add(param, Animator.StringToHash(param.ToString()));
      }
    }

    private class EqComparer<TEnum> : IEqualityComparer<TEnum> where TEnum : struct, Enum {
      private readonly Func<TEnum, int> _toInt;
      public EqComparer(Func<TEnum, int> enumToInt) => _toInt = enumToInt;
      public bool Equals(TEnum x, TEnum y) => _toInt(x) == _toInt(y);
      public int GetHashCode(TEnum obj) => _toInt(obj);
    }
  }
}