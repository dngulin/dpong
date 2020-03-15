using System;
using System.Collections.Generic;
using DPong.Game.UI;
using DPong.Level.Data;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Game.Screens.HotSeatGame {
  public interface IHotSeatMenuListener {
    void PlayClicked();
    void BackClicked();
    void NickNameChanged(Side side, string name);
    void InputSourceChanged(Side side, int srcIndex);
  }

  public class HotSeatGameMenu : SimpleMenu {
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _backButton;

    [SerializeField] private InputField _leftName;
    [SerializeField] private InputField _rightName;

    [SerializeField] private Dropdown _leftSources;
    [SerializeField] private Dropdown _rightSources;

    public void Init(IHotSeatMenuListener listener) {
      _playButton.onClick.AddListener(listener.PlayClicked);
      _backButton.onClick.AddListener(listener.BackClicked);

      _leftName.onEndEdit.AddListener(nick => listener.NickNameChanged(Side.Left, nick));
      _rightName.onEndEdit.AddListener(nick => listener.NickNameChanged(Side.Right, nick));

      _leftSources.onValueChanged.AddListener(index => listener.InputSourceChanged(Side.Left, index));
      _rightSources.onValueChanged.AddListener(index => listener.InputSourceChanged(Side.Right, index));
    }

    public void SetNickName(Side side, string nick) {
      switch (side) {
        case Side.Left:
          _leftName.text = nick;
          break;

        case Side.Right:
          _rightName.text = nick;
          break;

        default:
          throw new ArgumentOutOfRangeException(nameof(side), side, null);
      }
    }

    public void SetSources(Side side, IReadOnlyList<string> sources, int selected) {
      switch (side) {
        case Side.Left:
          SetSources(_leftSources, sources, selected);
          break;

        case Side.Right:
          SetSources(_rightSources, sources, selected);
          break;

        default:
          throw new ArgumentOutOfRangeException(nameof(side), side, null);
      }
    }

    private static void SetSources(Dropdown dropdown, IReadOnlyList<string> sources, int selected) {
      dropdown.options.Clear();

      foreach (var source in sources)
        dropdown.options.Add(new Dropdown.OptionData(source));

      dropdown.value = selected;
    }
  }
}