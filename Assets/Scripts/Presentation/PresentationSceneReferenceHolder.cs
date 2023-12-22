#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using UnityEngine;

namespace Presentation
{
    /// <summary>
    /// Keep in mind references stored here are only accessible AFTER Core scene is fully loaded up.
    /// Use, for example, <see cref="Controllers.PresentationMainController._coreSceneLoaded" /> to control the execution.
    /// </summary>
    class PresentationSceneReferenceHolder : MonoBehaviour
    {
        internal static Transform VfxContainer;
        internal static Transform AudioContainer;
        internal static Transform PlayerContainer;
        internal static Camera MainMenuCamera;
        internal static Camera GameplayCamera;
        internal static AudioSource MusicAudioSource;

        [SerializeField]
        Transform _vfxContainer;

        [SerializeField]
        Transform _audioContainer;

        [SerializeField]
        Transform _playerContainer;

        [SerializeField]
        Camera _mainCamera;

        [SerializeField]
        Camera _gameplayCamera;

        [SerializeField]
        AudioSource _musicAudioSource;

        void Awake()
        {
            VfxContainer = _vfxContainer;
            AudioContainer = _audioContainer;
            PlayerContainer = _playerContainer;
            MainMenuCamera = _mainCamera;
            GameplayCamera = _gameplayCamera;
            MusicAudioSource = _musicAudioSource;
        }
    }
}