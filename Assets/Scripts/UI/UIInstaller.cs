using Shared.DependencyInjector;
using UI.Controllers;
using UI.ViewModels;

namespace UI
{
    public class UIInstaller : AbstractInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UIViewModel>().AsSingle();
            Container.Bind<UIMainController>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputController>().AsSingle();
        }
    }
}