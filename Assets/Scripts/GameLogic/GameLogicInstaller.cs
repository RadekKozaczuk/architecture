using GameLogic.Controllers;
using GameLogic.ViewModels;
using Shared.DependencyInjector;

namespace GameLogic
{
    public class GameLogicInstaller : AbstractInstaller
    {
        public override void InstallBindings()
        {
            // controllers
            Container.BindInterfacesAndSelfTo<GameLogicMainController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameLogicViewModel>().AsSingle().Lazy();
        }
    }
}