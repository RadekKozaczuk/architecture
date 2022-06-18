using Boot.Controllers;
using UnityEngine;
using Zenject;

namespace Boot
{
    [DisallowMultipleComponent]
    class BootInstaller : MonoInstaller
    {
        public override void InstallBindings() => Container.BindInterfacesAndSelfTo<GameStateImplementationController>().AsSingle().NonLazy();
    }
}