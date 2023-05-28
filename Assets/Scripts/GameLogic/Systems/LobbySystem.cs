using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Signals;
using Shared;
using Shared.Systems;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace GameLogic.Systems
{
    /// <summary>
    /// This is system is responsible for creating a multiplayer lobby.
    /// </summary>
    static class LobbySystem
    {
        const float LobbyUpdateTimerMax = 1.1f;
        /// <summary>
        /// Maximum allowed lobby query rate is 1 per seconds. Executing queries faster will result in an error.
        /// </summary>
        const float LobbyQueryRate = 1.1f;

        /// <summary>
        /// This is only populated on host.
        /// </summary>
        static Lobby _hostLobby;

        /// <summary>
        /// This is populated both on host and on clients.
        /// </summary>
        static Lobby _joinedLobby;
        static float _heartbeatTimer;
        static float _lobbyUpdateTimer;

        static float _lobbyQueryTimer;
        static Action<List<(string lobbyCode, string lobbyName, int playerCount, int playerMax)>> _pendingLobbyQueryCallback;

        /// <summary>
        /// Indicates that the player changed it's name.
        /// </summary>
        static bool _lobbyIsDirty;

        internal static void CustomUpdate()
        {
            if (_lobbyQueryTimer > 0)
                _lobbyQueryTimer -= Time.deltaTime;
            else if (_pendingLobbyQueryCallback != null)
                ExecuteLobbyQueryCallback();

            if (_lobbyIsDirty)
            {
                UpdatePlayerName(GameLogicData.PlayerName);
            }

            if (_joinedLobby == null)
                return;

            HandleLobbyCallForUpdates();

            // host cannot be not null if joined is null that's wny it is below
            if (_hostLobby == null)
                return;

            HandleLobbyHeartbeat();
        }

        // lobbies are automatically turn inactive if the lobby does not receive any data
        // for 30 seconds
        internal static async Task<bool> CreateLobby(string lobbyName, int maxPlayers)
        {
            try
            {
                var options = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Player = new Player
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            {"playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, GameLogicData.PlayerName)}
                        }
                    }
                };

                _hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

                var players = new List<(string playerName, bool isHost)>();
                foreach (Player player in _hostLobby.Players)
                    // todo: name is an additional data and has to be transferred differently
                    players.Add((GameLogicData.PlayerName, player.Id == _hostLobby.HostId));

                SignalProcessor.SendSignal(new LobbyChangedSignal(_hostLobby.Name, players));
                Debug.Log("Created lobby " + _hostLobby.Name + " " + _hostLobby.MaxPlayers);
                return true;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                return true;
            }
        }

        internal static void RequestGetLobbies(Action<List<(string lobbyCode, string lobbyName, int playerCount, int playerMax)>> callback)
        {
            Assert.IsNotNull(callback, "callback function cannot be null.");

            _pendingLobbyQueryCallback = callback;
        }

        static async void ExecuteLobbyQueryCallback()
        {
            _lobbyQueryTimer = LobbyQueryRate;
            List<(string lobbyCode, string lobbyName, int playerCount, int playerMax)> lobbies = await QueryLobbies();
            _pendingLobbyQueryCallback.Invoke(lobbies);
            _pendingLobbyQueryCallback = null;
        }

        /// <summary>
        /// Rate limit for lobby querying is 1 query per second.
        /// Calling this method more often will result in an error.
        /// </summary>
        static async Task<List<(string lobbyCode, string lobbyName, int playerCount, int playerMax)>> QueryLobbies()
        {
            List<(string, string, int, int)> list = new();
            try
            {
                var options = new QueryLobbiesOptions {Count = 25};

                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);
                Debug.Log("Lobbies found: " + queryResponse.Results.Count);

                list.AddRange(queryResponse.Results.Select(lobby => (lobby.LobbyCode, lobby.Name, lobby.Players.Count, lobby.MaxPlayers)));
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

            return list;
        }

        internal static async void JoinLobby(string lobbyCode)
        {
            try
            {
                var options = new JoinLobbyByCodeOptions
                {
                    Player = new Player
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            {"playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, GameLogicData.PlayerName)}
                        }
                    }
                };

                Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
                Debug.Log("Joined lobby");
                PrintPlayers(lobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Queries available lobbies and join randomly selected one.
        /// </summary>
        internal static async void QuickJoinLobby()
        {
            try
            {
                await Lobbies.Instance.QuickJoinLobbyAsync();
                Debug.Log("Joined lobby");
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        static void PrintPlayers(Lobby lobby)
        {
            foreach (Player player in lobby.Players)
            {
                Debug.Log($"Player info, id: {player.Id}, name: {player.Data["playerName"].Value}");
            }
        }

        /// <summary>
        /// When we update lobby we need to update the local reference.
        /// And also we only need to update those values that we want.
        /// </summary>
        /// <param name="gameMode"></param>
        static async void UpdateLobbyGameMode(string gameMode)
        {
            try
            {
                _hostLobby = await Lobbies.Instance.UpdateLobbyAsync(
                    _hostLobby.Id, new UpdateLobbyOptions {Data = new Dictionary<string, DataObject>
                    {
                        {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode)}
                    }});
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        static async void UpdatePlayerName(string playerName)
        {
            try
            {
                // we should make isDirty pattern and call update when update happened
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(
                    _joinedLobby.Id,
                    AuthenticationService.Instance.PlayerId,
                    new UpdatePlayerOptions
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            {
                                "PlayerName",
                                new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)
                            }
                        }
                    });
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }

        /// <summary>
        /// a
        /// </summary>
        internal static async void LeaveLobby()
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }

        /// <summary>
        /// Only host can kick players. Host cannot kick himself.
        /// </summary>
        internal static async void KickPlayer(string playerId)
        {
            Assert.IsNotNull(playerId, $"Parameter {nameof(playerId)} cannot be null.");

            try
            {
                // todo: it should be taken from a parameter
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }

        /// <summary>
        /// Gives lobby host role to one of the players.
        /// </summary>
        /// <param name="playerId"></param>
        static async void GiveHost(string playerId)
        {
            // todo: it should be taken from a parameter
            try
            {
                _hostLobby = await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions {HostId = _joinedLobby.Players[1].Id});
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }

        /// <summary>
        /// Lobbies becomes inactive after 30s. In order to prevent that we have to ping it every 20s or so.
        /// </summary>
        static void HandleLobbyHeartbeat()
        {
            Assert.IsNotNull(_hostLobby, $"This method should not be called if {nameof(_hostLobby)} variable is null");

            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer < 0f)
            {
                _heartbeatTimer = 20;
                LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
            }
        }

        /// <summary>
        /// Every <see cref="LobbyUpdateTimerMax"/> it updates <see cref="_joinedLobby"/> reference with a new instance.
        /// </summary>
        static async void HandleLobbyCallForUpdates()
        {
            Assert.IsNotNull(_joinedLobby, $"This method should not be called if {nameof(_hostLobby)} variable is null");

            try
            {
                _lobbyUpdateTimer -= Time.deltaTime;
                if (_lobbyUpdateTimer < 0f)
                {
                    _lobbyUpdateTimer = LobbyUpdateTimerMax;
                    _joinedLobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);
                }
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }
    }
}