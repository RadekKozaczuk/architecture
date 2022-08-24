using Shared.DependencyInjector;

namespace Boot
{
    public class BootInstaller : AbstractInstaller
    {
        public override void InstallBindings() => Container.Bind<FlowController>().AsSingle();
    }
}