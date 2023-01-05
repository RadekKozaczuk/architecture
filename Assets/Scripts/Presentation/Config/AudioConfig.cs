using Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

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

        [FormerlySerializedAs("_music")]
        [InfoBox("Element order must match the Music enum.", InfoMessageType.None)]
        [SerializeField]
        internal AssetReferenceAudioClip[] Music;
    }
}