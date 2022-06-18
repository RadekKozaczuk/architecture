using Presentation.Views;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Presentation.Config
{
    [CreateAssetMenu(fileName = "VFXConfig", menuName = "Config/Presentation/VFXConfig")]
    class VFXConfig : ScriptableObject
    {
        [InfoBox("Order must match the VFX enum.", InfoMessageType.None)]
        [SerializeField]
        internal VFXView[] VfxPrefabs;
    }
}