using Presentation.Views;
using UnityEngine;

namespace Presentation
{
    public class PresentationSceneReferenceHolder : MonoBehaviour
    {
        public static AudioListener AudioListener;

        internal static Transform VfxContainer;
        internal static Light Light;
        internal static WolfView Wolf1;
        internal static WolfView Wolf2;
        internal static Transform Target;

        [SerializeField]
        AudioListener _audioListener;

        [SerializeField]
        Transform _vfxContainer;

        [SerializeField]
        Light _light;

        [SerializeField]
        WolfView _wolf1;

        [SerializeField]
        WolfView _wolf2;

        [SerializeField]
        Transform _target;

        void Awake()
        {
            AudioListener = _audioListener;
            VfxContainer = _vfxContainer;

            Light = _light;
            Wolf1 = _wolf1;
            Wolf2 = _wolf2;
            Target = _target;
        }
    }
}