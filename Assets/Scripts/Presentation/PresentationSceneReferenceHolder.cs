using Presentation.Views;
using UnityEngine;

namespace Presentation
{
    /// <summary>
    /// Keep in mind references stored here are only accessible AFTER Core scene is fully loaded up.
    /// Use, for example, <see cref="Controllers.PresentationMainController._coreSceneLoaded" /> to control the execution.
    /// </summary>
    class PresentationSceneReferenceHolder : MonoBehaviour
    {
        internal static AudioListener AudioListener;
        internal static Transform VfxContainer;
        internal static Light Light;
        internal static Transform Target;

        [SerializeField]
        AudioListener _audioListener;

        [SerializeField]
        Transform _vfxContainer;

        [SerializeField]
        Light _light;

        [SerializeField]
        Transform _target;

        void Awake()
        {
            AudioListener = _audioListener;
            VfxContainer = _vfxContainer;

            Light = _light;
            Target = _target;
        }
    }
}