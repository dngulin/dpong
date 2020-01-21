using System;
using DPong.Core.UI.Helpers;
using UnityEngine;

namespace DPong.Core.UI.Holder {
  public class UIHolder : MonoBehaviour {
    public enum State {
      Open,
      Opened,
      Close,
      Closed
    }

    public enum Param {
      IsOpened
    }

    internal static readonly AnimatorHashes<State, Param> AnimatorHashes =
      new AnimatorHashes<State, Param>(state => (int) state, param => (int) param);

    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _contentRoot;

    private State _state;

    public event Action OnOpenStart;
    public event Action OnOpenFinish;

    public event Action OnHideStart;
    public event Action OnHideFinish;

    internal void InternalInit(bool visible) {
      _state = visible ? State.Opened : State.Closed;
      _animator.Play(AnimatorHashes.States[_state]);
      _animator.SetBool(AnimatorHashes.Params[Param.IsOpened], visible);
      _animator.Update(0f);

      _animator.GetBehaviour<UIHolderStateMachineBehaviour>().OnStateEntered += state => {
        _state = state;
        switch (_state) {
          case State.Open:
            OnOpenStart?.Invoke();
            break;
          case State.Opened:
            OnOpenFinish?.Invoke();
            break;
          case State.Close:
            OnHideStart?.Invoke();
            break;
          case State.Closed:
            OnHideFinish?.Invoke();
            break;

          default:
            throw new ArgumentOutOfRangeException();
        }
      };
    }

    public Transform ContentRoot => _contentRoot;

    public void Show() => _animator.SetBool(AnimatorHashes.Params[Param.IsOpened], true);

    public void Hide() => _animator.SetBool(AnimatorHashes.Params[Param.IsOpened], false);
  }
}