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
        [MaxValue(10)]
        [SerializeField]
        internal float PlayerSpeed = 5;
    }
}