using DPong.Loader.UI;
using UnityEngine;

namespace DPong.Loader {
  [CreateAssetMenu(fileName = "LoaderResources", menuName = "DPONG/LoaderResources")]
  public class MenuPrefabs : ScriptableObject {
    public MainMenu MainMenu;
    public HotSeatMenu HotSeatMenu;
    public NetworkGameMenu NetworkGameMenu;
  }
}