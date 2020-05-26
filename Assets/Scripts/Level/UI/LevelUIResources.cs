using UnityEngine;
using UnityEngine.Serialization;

namespace DPong.Level.UI
{
    [CreateAssetMenu(fileName = "LevelUIResources", menuName = "DPong/LevelUIResources", order = 0)]
    public class LevelUIResources : ScriptableObject
    {
        [FormerlySerializedAs("PauseButton")] public PausePanel pausePanel;
    }
}