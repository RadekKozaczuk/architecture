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

        [Min(0)]
        [MaxValue(10)]
        [SerializeField]
        internal float PlayerJumpForce = 8;

        [Min(0)]
        [MaxValue(10)]
        [SerializeField]
        internal float PlayerInteractDistance = 8;

        [Min(1)]
        [MaxValue(3)]
        [SerializeField]
        internal float PlayerGroundDrag = 1;

        [Min(1)]
        [MaxValue(5)]
        [SerializeField]
        internal float PlayerAirDrag = 5;
    }
}