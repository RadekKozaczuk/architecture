using GameLogic.Config;
using GameLogic.Controllers;
using GameLogic.ViewModels;
using UnityEngine;
using Zenject;

namespace GameLogic
{
    public class GameLogicInstaller : MonoInstaller
    {
        [SerializeField]
        GameplayConfig _gameplayConfig;

        public override void InstallBindings()
        {
            // configs
            Container.BindInstance(_gameplayConfig);

            // controllers
            Container.BindInterfacesAndSelfTo<GameLogicMainController>().AsSingle();
            Container.Bind<GameLogicViewModel>().AsSingle();
        }
    }
}