using DPong.Game.UI;
using DPong.UI;
using UnityEngine;

namespace DPong.Game {
  [CreateAssetMenu(fileName = "LoaderResources", menuName = "DPONG/GameResources")]
  public class GameResources : ScriptableObject {
    public UISystemResources UISystemResources;

    public MainMenu MainMenuPrefab;
    public HotSeatMenu HotSeatMenuPrefab;
    public NetworkGameMenu NetworkGameMenuPrefab;
  }
}