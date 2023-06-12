using System;
using Common;
using Common.Signals;
using Common.Systems;
using ControlFlow.DependencyInjector.Attributes;
using ControlFlow.DependencyInjector.Interfaces;
using GameLogic.Controllers;
using GameLogic.Systems;
using JetBrains.Annotations;
using Presentation.ViewModels;
using Shared;
using Shared.Systems;
using UnityEngine.Scripting;

namespace GameLogic.ViewModels
{
    [UsedImplicitly]
    public partial class GameLogicViewModel : IInitializable
    {
        public static bool SaveFileExist => SaveLoadSystem.SaveFileExist;

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

        public static void MainMenuOnEntry() => CommonData.PlayerName = Utils.GenerateRandomString(7);

        public static void MainMenuOnExit() { }

        public static void GameplayOnEntry()
        {
            if (CommonData.LoadRequested)
                SaveLoadSystem.LoadGame();
        }

        public static void GameplayOnExit() { }

        public static void SaveGame() => SaveLoadSystem.SaveGame();

        public static void LoadGame()
        {
        }

        /// <summary>
        /// If the instance hosted a lobby, the lobby will be deleted.
        /// </summary>
        public static void QuitGame() => LobbySystem.SignOut();

        public static void WinMission() => SignalProcessor.SendSignal(new MissionCompleteSignal());

        public static void FailMission() => SignalProcessor.SendSignal(new MissionFailedSignal());
    }
}