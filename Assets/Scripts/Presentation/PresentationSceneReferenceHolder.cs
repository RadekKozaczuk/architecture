using UnityEngine;

namespace Presentation
{
    public class PresentationSceneReferenceHolder : MonoBehaviour
    {
        public static AudioListener AudioListener;

        internal static Transform VfxContainer;
        internal static Light Light;

        [SerializeField]
        AudioListener _audioListener;

        [SerializeField]
        Transform _vfxContainer;

        [SerializeField]
        Light _light;

        void Awake()
        {
            AudioListener = _audioListener;
            VfxContainer = _vfxContainer;
            Light = _light;
        }
    }
}