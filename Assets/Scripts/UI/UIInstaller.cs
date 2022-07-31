using Shared.DependencyInjector;
using UI.Controllers;
using UI.ViewModels;

namespace UI
{
    public class UIInstaller : AbstractInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UIViewModel>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UIMainController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<InputController>().AsSingle();
        }
    }
}