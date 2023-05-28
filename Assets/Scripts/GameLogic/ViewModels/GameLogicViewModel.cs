using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameLogic.ViewModels
{
    [UsedImplicitly]
    public class GameLogicViewModel : IInitializable
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

        public static void MainMenuOnEntry() { }

        public static void MainMenuOnExit() { }

        public static void GameplayOnEntry()
        {
            if (CommonData.LoadRequested)
                SaveLoadSystem.LoadGame();

            GameLogicData.PlayerName = Utils.GenerateRandomString(5, 9);
        }

        public static void GameplayOnExit() { }

        public static void SaveGame()
        {
            SaveLoadSystem.SaveGame();
        }

        public static void LoadGame()
        {
        }

        public static void WinMission() => SignalProcessor.SendSignal(new MissionCompleteSignal());

        public static void FailMission() => SignalProcessor.SendSignal(new MissionFailedSignal());

        public static void NetworkSetup() => NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;

        public static void RequestGetLobbies(Action<List<(string lobbyCode, string lobbyName, int playerCount, int playerMax)>> callback)
        {
            LobbySystem.RequestGetLobbies(callback);
        }

        public static void JoinLobby(string lobbyCode)
        {
            LobbySystem.JoinLobby(lobbyCode);
        }

        /// <summary>
        /// Returns true if the lobby was successfully created.
        /// </summary>
        public static async Task<bool> CreateLobby(string lobbyName, int maxPlayers) => await LobbySystem.CreateLobby(lobbyName, maxPlayers);

        public static void KickPlayer(string playerId) => LobbySystem.KickPlayer(playerId);

        public static void LeaveLobby() => LobbySystem.LeaveLobby();

        // todo: should be moved to MainController probably
        static void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            // Your approval logic determines the following values
            response.Approved = true;
            response.CreatePlayerObject = true;

            // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
            response.PlayerPrefabHash = null;

            // Position to spawn the player object (if null it uses default of Vector3.zero)
            response.Position = Vector3.zero;

            // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
            response.Rotation = Quaternion.identity;

            // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
            // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
            response.Reason = "Some reason for not approving the client";

            // If additional approval steps are needed, set this to true until the additional steps are complete
            // once it transitions from true to false the connection approval response will be processed.
            response.Pending = false;
        }
    }
}