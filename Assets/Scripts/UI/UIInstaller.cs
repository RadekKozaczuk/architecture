using Shared.DependencyInjector.Install;
using UI.Controllers;

namespace UI
{
    public class UIInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UIMainController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<InputHandler>().AsSingle();
        }
    }
}