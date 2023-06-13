using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Dtos;
using Common.Enums;
using Common.Systems;
using GameLogic.Systems;
using Unity.Netcode;
using UnityEngine;

namespace GameLogic.ViewModels
{
    public partial class GameLogicViewModel
    {
        public static void RequestGetLobbies(Action<LobbyDto[]> callback)
            => LobbySystem.RequestGetLobbies(callback);

        public static void JoinLobbyById(string lobbyId, Action<string, List<(string playerName, string playerId, bool isHost)>> callback)
            => LobbySystem.JoinLobbyById(lobbyId, callback);

        public static void JoinLobbyByCode(string lobbyCode) => LobbySystem.JoinLobbyByCode(lobbyCode);

        public static void QuickJoinLobby() => LobbySystem.QuickJoinLobby();

        /// <summary>
        /// Returns true if the lobby was successfully created.
        /// </summary>
        public static async Task<(bool, string)> CreateLobby(string lobbyName, int maxPlayers) => await LobbySystem.CreateLobby(lobbyName, maxPlayers);

        public static void KickPlayer(string playerId) => LobbySystem.KickPlayer(playerId);

        public static void LeaveLobby() => LobbySystem.LeaveLobby();

        public static async void StartGame()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            await LobbySystem.StartGame_Host();

            // request state change must happen after NetworkManager.Singleton.StartHost();
            GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CommonData.CurrentLevel});
        }

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