using DPong.Game.UI;
using UnityEngine;

namespace DPong.Game {
  [CreateAssetMenu(fileName = "LoaderResources", menuName = "DPONG/LoaderResources")]
  public class MenuPrefabs : ScriptableObject {
    public MainMenu MainMenu;
    public HotSeatMenu HotSeatMenu;
    public NetworkGameMenu NetworkGameMenu;
  }
}