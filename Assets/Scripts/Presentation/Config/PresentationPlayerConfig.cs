using Sirenix.OdinInspector;
using UnityEngine;

namespace Presentation.Config
{
    [CreateAssetMenu(fileName = "PresentationPlayerConfig", menuName = "Config/Presentation/PresentationPlayerConfig")]
    class PresentationPlayerConfig : ScriptableObject
    {
        [SerializeField]
        internal GameObject PlayerPrefab;

        [Min(0)]
        [MaxValue(1000)]
        [SerializeField]
        [InfoBox("In Unity units per second.", InfoMessageType.None)]
        internal float PlayerSpeed = 100;
    }
}