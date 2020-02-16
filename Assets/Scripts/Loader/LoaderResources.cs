using DPong.Loader.UI;
using UnityEngine;

namespace DPong.Loader {
  [CreateAssetMenu(fileName = "LoaderResources", menuName = "DPONG/LoaderResources")]
  public class LoaderResources : ScriptableObject {
    public MainMenu MainMenu;
    public LocalGameMenu LocalGameMenu;
    public NetworkGameMenu NetworkGameMenu;
  }
}