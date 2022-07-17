using Shared.DependencyInjector.Install;
using UI.Controllers;
using UI.ViewModels;

namespace UI
{
    public class UIInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UIViewModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UIMainController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<InputController>().AsSingle();
        }
    }
}