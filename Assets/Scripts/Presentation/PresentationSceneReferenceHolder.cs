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
        internal static Light Light;
        internal static Camera MainMenuCamera;
        internal static Camera GameplayCamera;
        internal static AudioSource MusicAudioSource;

        [SerializeField]
        Transform _vfxContainer;

        [SerializeField]
        Light _light;

        [SerializeField]
        Camera _mainCamera;

        [SerializeField]
        Camera _gameplayCamera;

        [SerializeField]
        AudioSource _musicAudioSource;

        void Awake()
        {
            VfxContainer = _vfxContainer;
            Light = _light;
            MainMenuCamera = _mainCamera;
            GameplayCamera = _gameplayCamera;
            MusicAudioSource = _musicAudioSource;
        }
    }
}