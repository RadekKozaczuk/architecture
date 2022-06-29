using GameLogic.Controllers;
using GameLogic.ViewModels;
using Shared.DependencyInjector.Install;

namespace GameLogic
{
    public class GameLogicInstaller : Installer
    {
        public override void InstallBindings()
        {
            // controllers
            Container.BindInterfacesAndSelfTo<GameLogicMainController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameLogicViewModel>().AsSingle().Lazy();
        }
    }
}