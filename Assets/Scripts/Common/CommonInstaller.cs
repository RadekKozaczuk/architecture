using Common.Config;
using UnityEngine;
using Zenject;

namespace Common
{
    public class CommonInstaller : MonoInstaller
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField]
        DebugConfig _debugConfig;
#endif

        [SerializeField]
        PlayerConfig _playerConfig;

        public override void InstallBindings()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Container.BindInstance(_debugConfig);
#endif

            Container.BindInstance(_playerConfig);
        }
    }
}