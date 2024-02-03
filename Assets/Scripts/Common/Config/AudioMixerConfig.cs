#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using UnityEngine;
using UnityEngine.Audio;

namespace Common.Config
{
    [CreateAssetMenu(fileName = "AudioMixerConfig", menuName = "Config/Common/AudioMixerConfig")]
    public class AudioMixerConfig : ScriptableObject
    {
        public AudioMixer AudioMixer;
        public AudioMixerGroup AudioMixerSFX;
    }
}
