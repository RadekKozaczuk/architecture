using Presentation.Views;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Presentation.Config
{
    [CreateAssetMenu(fileName = "VFXConfig", menuName = "Config/Presentation/VFXConfig")]
    class VFXConfig : ScriptableObject
    {
        [InfoBox("Visual Effects done in Unity's Particle System. The order of elements must match the VFX enum.", InfoMessageType.None)]
        [SerializeField]
        internal ParticleEffectView[] ParticleEffects;

        [InfoBox("Visual Effects done in Unity's VFX Graph. The order of elements must match the VFX enum.", InfoMessageType.None)]
        [SerializeField]
        internal GameObject[] VisualEffects; // todo: prefab and actual type to be done
    }
}