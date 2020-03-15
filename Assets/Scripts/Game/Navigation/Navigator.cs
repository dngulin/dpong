using System;
using System.Collections.Generic;

namespace DPong.Game.Navigation {
  public class Navigator {
    private readonly Stack<INavigationPoint> _navigationStack = new Stack<INavigationPoint>();

    private uint _lastToken;
    private readonly Dictionary<uint, INavigationPoint> _registeredPoints = new Dictionary<uint, INavigationPoint>();

    public NavigationToken Register(INavigationPoint point) {
      var token = new NavigationToken(_lastToken++);
      _registeredPoints.Add(token.Id, point);
      return token;
    }

    public void Clear() => _registeredPoints.Clear();

    public void Enter(NavigationToken token) {
      if (!_registeredPoints.TryGetValue(token.Id, out var point))
        throw new InvalidOperationException($"Navigation point with token '{token.Id}' does not registered!");

      Enter(point);
    }

    public void Enter(INavigationPoint point) {
      if (_navigationStack.Contains(point))
        throw new InvalidOperationException("Try to place the navigation point to stack twice!");

      if (_navigationStack.Count > 0)
        _navigationStack.Peek().Suspend();

      point.Enter();
      _navigationStack.Push(point);
    }

    public void Exit(INavigationPoint point) {
      if (_navigationStack.Peek() != point)
        throw new InvalidOperationException("Try to leave from not top level point!");

      point.Exit();
      _navigationStack.Pop();

      if (_navigationStack.Count > 0)
        _navigationStack.Peek().Resume();
    }
  }
}