#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using UnityEngine;
using UnityEngine.Audio;

namespace Common.Config
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Config/Common/AudioConfig")]
    public class AudioConfig : ScriptableObject
    {
        public AudioMixer AudioMixer;
        public AudioMixerGroup AudioMixerSFX;
    }
}
