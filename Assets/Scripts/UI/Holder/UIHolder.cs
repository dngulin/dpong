using System;
using DPong.Common;
using UnityEngine;

namespace DPong.UI.Holder {
  public class UIHolder : MonoBehaviour, IUserInterface {
    public enum State {
      Open,
      Opened,
      Close,
      Closed
    }

    public enum Param {
      IsOpened
    }

    internal static readonly AnimatorHashes<State, Param> AnimatorHashes = new AnimatorHashes<State, Param>();

    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _contentRoot;
    [SerializeField] private CanvasGroup _canvasGroup;

    private State _state;

    private bool _initialized;

    public event Action OnOpenStart;
    public event Action OnOpenFinish;

    public event Action OnHideStart;
    public event Action OnHideFinish;

    public void SetInitialVisibility(bool visible) {
      if (_initialized)
        throw new InvalidOperationException();

      _initialized = true;
      _state = visible ? State.Opened : State.Closed;

      _canvasGroup.alpha = visible ? 1 : 0;
      _canvasGroup.interactable = visible;

      _animator.Play(AnimatorHashes.GetStateHash(_state));
      _animator.SetBool(AnimatorHashes.GetParameterHash(Param.IsOpened), visible);
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

    public void Show() => _animator.SetBool(AnimatorHashes.GetParameterHash(Param.IsOpened), true);

    public void Hide() => _animator.SetBool(AnimatorHashes.GetParameterHash(Param.IsOpened), false);
  }
}