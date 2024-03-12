#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using Common;
using Common.Enums;
using Common.Systems;
using ControlFlow.DependencyInjector;
using GameLogic.Controllers;
using GameLogic.Systems;
using JetBrains.Annotations;
using Presentation.ViewModels;
using Shared;
using Unity.Netcode;
using UnityEngine.Scripting;
using Random = UnityEngine.Random;

namespace GameLogic.ViewModels
{
    [UsedImplicitly]
    public partial class GameLogicViewModel
    {
        public static bool SaveFileExist => SaveLoadSystem.SaveFileExist;

        [Inject]
        static readonly GameLogicMainController _gameLogicMainController;

        [Preserve]
        GameLogicViewModel() { }

        public static void CustomUpdate()
        {
            _gameLogicMainController.CustomUpdate();
            PresentationViewModel.CustomUpdate();
        }

        public static void CustomFixedUpdate() => _gameLogicMainController.CustomFixedUpdate();

        public static void CustomLateUpdate() => _gameLogicMainController.CustomLateUpdate();

        /// <summary>
        /// Result retrieval (processing) that should be handled in the callback function.
        /// </summary>
        public static void ValidatePlayer(string accessCode, Action<bool> callback) =>
            StaticCoroutine.StartStaticCoroutine(JsonSystem.ValidateProfileAsync(accessCode, callback));

        public static void BootingOnExit() => PersistentStorageSystem.Initialize();

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

        public static void SaveVolumeSettings(int music, int sound) => PersistentStorageSystem.SaveVolumeSettings(music, sound);

        public static (int music, int sound) LoadVolumeSettings() => PersistentStorageSystem.LoadVolumeSettings();

        /// <summary>
        /// If the instance hosted a lobby, the lobby will be deleted.
        /// </summary>
        public static void QuitGame() => LobbySystem.SignOut();

        /*public static void WinMission() => Signals.MissionComplete();

        public static void FailMission() => Signals.MissionFailed();*/
    }
}