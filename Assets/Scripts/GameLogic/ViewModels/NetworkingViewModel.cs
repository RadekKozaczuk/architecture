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

        public static async Task JoinServer() => await LobbySystem.JoinServer();

        /// <summary>
        /// If the lobby was successfully created it returns true and the first player's id, false and null otherwise.
        /// </summary>
        public static async Task<(bool, string)> CreateLobby(string lobbyName, int maxPlayers) => await LobbySystem.CreateLobby(lobbyName, maxPlayers);

        /// <summary>
        /// Only host can kick players. Host cannot kick himself.
        /// </summary>
        public static void KickPlayer(string playerId) => LobbySystem.KickPlayer(playerId);

        public static void LeaveLobby() => LobbySystem.LeaveLobby();

        public static async void StartGame()
        {
            await LobbySystem.StartGame_Host();
        }
    }
}