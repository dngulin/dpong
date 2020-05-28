using UnityEngine;
using UnityEngine.Serialization;

namespace DPong.Level.UI
{
    [CreateAssetMenu(fileName = "LevelUIResources", menuName = "DPong/LevelUIResources", order = 0)]
    public class LevelUIResources : ScriptableObject
    {
        public PausePanel PausePanel;
        public PauseDialog PauseDialog;
    }
}