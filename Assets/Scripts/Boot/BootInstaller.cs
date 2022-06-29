using Boot.Controllers;
using Shared.DependencyInjector.Install;

namespace Boot
{
    public class BootInstaller : Installer
    {
        public override void InstallBindings() => Container.BindInterfacesAndSelfTo<FlowController>().AsSingle().NonLazy();
    }
}