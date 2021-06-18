using System;
using System.Collections.Generic;
using DPong.Level.Data;
using DPong.Meta.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DPong.Meta.Screens.HotSeatMenu {
  public class HotSeatGameMenu : SimpleMenu {
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _backButton;

    [SerializeField] private InputField _leftName;
    [SerializeField] private InputField _rightName;

    [SerializeField] private Dropdown _leftSources;
    [SerializeField] private Dropdown _rightSources;

    public readonly Queue<HotSeatMenuEvent> Events = new Queue<HotSeatMenuEvent>();

    private void Awake() {
      _playButton.onClick.AddListener(() => Events.Enqueue(HotSeatMenuEvent.Play()));
      _backButton.onClick.AddListener(() => Events.Enqueue(HotSeatMenuEvent.Back()));

      _leftName.onEndEdit.AddListener(nick => Events.Enqueue(HotSeatMenuEvent.NickChanged(Side.Left, nick)));
      _rightName.onEndEdit.AddListener(nick => Events.Enqueue(HotSeatMenuEvent.NickChanged(Side.Right, nick)));

      _leftSources.onValueChanged.AddListener(index => Events.Enqueue(HotSeatMenuEvent.InputSrcChanged(Side.Left, index)));
      _rightSources.onValueChanged.AddListener(index => Events.Enqueue(HotSeatMenuEvent.InputSrcChanged(Side.Right, index)));
    }

    public void SetPlayerName(Side side, string playerName) {
      switch (side) {
        case Side.Left:
          _leftName.text = playerName;
          break;

        case Side.Right:
          _rightName.text = playerName;
          break;

        default:
          throw new ArgumentOutOfRangeException(nameof(side), side, null);
      }
    }

    public void SetInputSources(Side side, IReadOnlyList<string> sources, int selected) {
      switch (side) {
        case Side.Left:
          SetInputSources(_leftSources, sources, selected);
          break;

        case Side.Right:
          SetInputSources(_rightSources, sources, selected);
          break;

        default:
          throw new ArgumentOutOfRangeException(nameof(side), side, null);
      }
    }

    private static void SetInputSources(Dropdown dropdown, IReadOnlyList<string> sources, int selected) {
      dropdown.options.Clear();

      foreach (var source in sources)
        dropdown.options.Add(new Dropdown.OptionData(source));

      dropdown.value = selected;
    }
  }
}