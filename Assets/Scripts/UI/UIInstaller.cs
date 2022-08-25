using Shared.DependencyInjector;
using UI.Controllers;
using UI.ViewModels;

namespace UI
{
    public class UIInstaller : AbstractInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<UIViewModel>().AsSingle();
            Container.Bind<UIMainController>().AsSingle();
            Container.Bind<InputController>().AsSingle();
        }
    }
}