#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable

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
    }
}