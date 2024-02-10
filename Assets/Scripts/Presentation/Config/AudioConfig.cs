#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace Presentation.Config
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Config/Presentation/AudioConfig")]
    class AudioConfig : ScriptableObject
    {
        [SerializeField]
        internal AudioSource AudioSourcePrefab;

        [InfoBox("Element order must match the Sound enum.", InfoMessageType.None)]
        [SerializeField]
        internal AudioClip[] Sounds;

        [InfoBox("Element order must match the Music enum.", InfoMessageType.None)]
        [SerializeField]
        internal AssetReferenceAudioClip[] Music;

        [SerializeField]
        internal AudioMixer AudioMixer;

        [SerializeField]
        internal AudioMixerGroup AudioMixerSound;
    }
}