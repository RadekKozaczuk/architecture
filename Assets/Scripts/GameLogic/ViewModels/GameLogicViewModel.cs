using System;
using Common.Systems;
using GameLogic.Controllers;
using JetBrains.Annotations;
using Presentation.ViewModels;
using Shared;
using Shared.DependencyInjector;
using Shared.DependencyInjector.Attributes;
using Shared.DependencyInjector.Interfaces;

namespace GameLogic.ViewModels
{
    [UsedImplicitly]
    public class GameLogicViewModel : IInitializable
    {
        static GameLogicViewModel _instance;

        [Inject]
        GameLogicMainController _gameLogicMainController;

        public void Initialize() => _instance = this;

        public static void CustomUpdate()
        {
            _instance._gameLogicMainController.CustomUpdate();
            PresentationViewModel.CustomUpdate();
        }

        public static void CustomFixedUpdate() => _instance._gameLogicMainController.CustomFixedUpdate();

        public static void CustomLateUpdate() => _instance._gameLogicMainController.CustomLateUpdate();

        /// <summary>
        /// Result retrieval (processing) that should be handled in the callback function.
        /// </summary>
        public static void ValidatePlayer(string accessCode, Action<bool> callback) =>
            StaticCoroutine.StartStaticCoroutine(JsonSystem.ValidateProfileAsync(accessCode, callback));
    }
}