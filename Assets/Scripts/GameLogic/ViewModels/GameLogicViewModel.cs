#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable
using System;
using Common;
using Common.Enums;
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
using Unity.Netcode;
using UnityEngine.Scripting;
using Random = UnityEngine.Random;

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

        public static void MainMenuOnEntry() => CommonData.PlayerName = Utils.GenerateRandomString(Random.Range(5, 9));

        public static void MainMenuOnExit()
        {
            // if client and multiplayer and client not started - start client
            if (CommonData.IsMultiplayer && !NetworkManager.Singleton.IsClient)
                NetworkManager.Singleton.StartClient();
        }

        public static void GameplayOnEntry()
        {
            bool loadGameRequested = (bool)GameStateSystem.GetTransitionParameter(StateTransitionParameter.LoadGameRequested)!;
            if (loadGameRequested)
                SaveLoadSystem.LoadGame();
        }

        public static void GameplayOnExit() { }

        public static void SaveGame() => SaveLoadSystem.SaveGame();

        public static void LoadGame() { }

        /// <summary>
        /// If the instance hosted a lobby, the lobby will be deleted.
        /// </summary>
        public static void QuitGame() => LobbySystem.SignOut();

        public static void WinMission() => SignalProcessor.SendSignal(new MissionCompleteSignal());

        public static void FailMission() => SignalProcessor.SendSignal(new MissionFailedSignal());
    }
}