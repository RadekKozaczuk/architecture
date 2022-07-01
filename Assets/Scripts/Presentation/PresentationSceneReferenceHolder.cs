using Presentation.Views;
using UnityEngine;

namespace Presentation
{
    public class PresentationSceneReferenceHolder : MonoBehaviour
    {
        public static AudioListener AudioListener;

        internal static Transform VfxContainer;
        internal static Light Light;
        internal static EnemyView Enemy;
        internal static Transform Target;

        [SerializeField]
        AudioListener _audioListener;

        [SerializeField]
        Transform _vfxContainer;

        [SerializeField]
        Light _light;
        
        [SerializeField]
        EnemyView _enemy;
        
        [SerializeField]
        Transform _target;

        void Awake()
        {
            AudioListener = _audioListener;
            VfxContainer = _vfxContainer;
            
            Light = _light;
            Enemy = _enemy;
            Target = _target;
        }
    }
}