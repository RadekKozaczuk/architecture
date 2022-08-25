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
            Container.Bind<GameLogicMainController>().AsSingle();
            Container.Bind<GameLogicViewModel>().AsSingle();
        }
    }
}