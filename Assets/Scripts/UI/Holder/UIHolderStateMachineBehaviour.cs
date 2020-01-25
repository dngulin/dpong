using System;
using UnityEngine;

namespace DPong.UI.Holder {
  public class UIHolderStateMachineBehaviour : StateMachineBehaviour {
    public event Action<UIHolder.State> OnStateEntered;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
      if (UIHolder.AnimatorHashes.HashToStateMap.TryGetValue(stateInfo.shortNameHash, out var state))
        OnStateEntered?.Invoke(state);
    }
  }
}