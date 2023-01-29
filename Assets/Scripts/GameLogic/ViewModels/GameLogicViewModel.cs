using System;
using Common.Systems;
using ControlFlow.DependencyInjector.Attributes;
using ControlFlow.DependencyInjector.Interfaces;
using GameLogic.Controllers;
using JetBrains.Annotations;
using Presentation.ViewModels;
using Shared;
using UnityEngine.Scripting;

namespace GameLogic.ViewModels
{
    [UsedImplicitly]
    public class GameLogicViewModel : IInitializable
    {
        static GameLogicViewModel _instance;

        [Inject]
        readonly GameLogicMainController _gameLogicMainController;

        [Preserve]
        GameLogicViewModel() { }

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

        public static void BootingOnExit() { }

        public static void MainMenuOnEntry() { }

        public static void MainMenuOnExit() { }

        public static void GameplayOnEntry() { }

        public static void GameplayOnExit() { }

        public static void SaveGame() { }

        public static void LoadGame() { }
    }
}