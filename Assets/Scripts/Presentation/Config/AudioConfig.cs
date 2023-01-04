using Common.Enums;
using Shared;
using Sirenix.OdinInspector;
using UnityEngine;

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
        AssetReferenceAudioClip[] _music;

        AudioClip[] _loadedMusic;

        /// <summary>
        /// Loads music asset into memory.
        /// </summary>
        internal void LoadMusic(Music music)
        {
            int id = (int)music;
            _music[id].LoadAssetAsync<AudioClip>().Completed += asyncOperationHandle => _loadedMusic[id] = asyncOperationHandle.Result;
        }

        /// <summary>
        /// Unloads music asset from memory.
        /// </summary>
        internal void UnloadMusic(Music music)
        {
            int id = (int)music;
            _loadedMusic[id] = null;
            _music[id].ReleaseAsset();
        }

        /// <summary>
        /// Gets music asset reference.
        /// </summary>
        internal AudioClip GetMusic(Music music) => _loadedMusic[(int)music];
    }
}