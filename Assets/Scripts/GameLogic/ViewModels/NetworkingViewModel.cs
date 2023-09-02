using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Dtos;
using GameLogic.Systems;

namespace GameLogic.ViewModels
{
    public partial class GameLogicViewModel
    {
        public static void RequestGetLobbies(Action<LobbyDto[]> callback) => LobbySystem.RequestGetLobbies(callback);

        public static void JoinLobbyById(string lobbyId, Action<string, string, List<(string playerName, string playerId, bool isHost)>> callback) =>
            LobbySystem.JoinLobbyById(lobbyId, callback);

        public static void JoinLobbyByCode(string lobbyCode, Action<string, string, List<(string playerName, string playerId, bool isHost)>> callback) =>
            LobbySystem.JoinLobbyByCode(lobbyCode, callback);

        public static void RejoinToLobby(string lobbyId, Action<string, string, List<(string playerName, string playerId, bool isHost)>> callback) =>
            LobbySystem.RejoinToLobby(lobbyId, callback);

        public static void QuickJoinLobby() => LobbySystem.QuickJoinLobby();

        /// <summary>
        /// If the lobby was successfully created it returns true and the first player's id, false and null otherwise.
        /// </summary>
        public static async Task<(bool, string, string)> CreateLobby(string lobbyName, int maxPlayers) =>
            await LobbySystem.CreateLobby(lobbyName, maxPlayers);

        /// <summary>
        /// Only host can kick players. Host cannot kick himself.
        /// </summary>
        public static void KickPlayer(string playerId) => LobbySystem.KickPlayer(playerId);

        /// <summary>
        /// Only host can gives lobby host role to one of the players.
        /// </summary>
        public static void GiveHost(string playerId) => LobbySystem.GiveHost(playerId);

        public static void LeaveLobby() => LobbySystem.LeaveLobby();

        public static void RemovePlayerFromLobby(string lobbyId) => LobbySystem.RemovePlayerFromLobby(lobbyId);

        public static async void StartGame() => await LobbySystem.StartGame_Host();

        public static void JoinVoiceChat() => VoiceChatSystem.Login();
    }
}