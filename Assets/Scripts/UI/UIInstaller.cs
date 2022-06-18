using UI.Config;
using UI.Controllers;
using UnityEngine;
using Zenject;

namespace UI
{
    class UIInstaller : MonoInstaller
    {
        [SerializeField]
        UIConfig _uiConfig;

        public override void InstallBindings()
        {
            // configs
            Container.BindInstance(_uiConfig);

            Container.BindInterfacesAndSelfTo<UIMainController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<InputHandler>().AsSingle();
        }
    }
}