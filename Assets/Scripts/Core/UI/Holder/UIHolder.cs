using System;
using DPong.Common;
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
    [SerializeField] private CanvasGroup _canvasGroup;

    private State _state;

    public event Action OnOpenStart;
    public event Action OnOpenFinish;

    public event Action OnHideStart;
    public event Action OnHideFinish;

    internal void InternalInit(bool visible) {
      _state = visible ? State.Opened : State.Closed;

      _canvasGroup.alpha = visible ? 1 : 0;
      _canvasGroup.interactable = visible;

      _animator.Play(AnimatorHashes.States[_state]);
      _animator.SetBool(AnimatorHashes.Params[Param.IsOpened], visible);
      _animator.Update(0f);

      _animator.GetBehaviour<UIHolderStateMachineBehaviour>().OnStateEntered += state => {
        _state = state;
        switch (_state) {
          case State.Open:
            _canvasGroup.alpha = 1;
            OnOpenStart?.Invoke();
            break;
          case State.Opened:
            _canvasGroup.interactable = true;
            OnOpenFinish?.Invoke();
            break;
          case State.Close:
            _canvasGroup.interactable = false;
            OnHideStart?.Invoke();
            break;
          case State.Closed:
            _canvasGroup.alpha = 0;
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