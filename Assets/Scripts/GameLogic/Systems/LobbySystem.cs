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
        static bool IsHost => AuthenticationService.Instance.PlayerId == Lobby.HostId;

        /// <summary>
        /// In seconds. Heartbeat is limited to 5 requests per 30 seconds.
        /// </summary>
        const float HeartbeatRate = 25f;
        const float LobbyUpdateTimerMax = 1.1f;
        /// <summary>
        /// Maximum allowed lobby query rate is 1 per seconds. Executing queries faster will result in an error.
        /// </summary>
        const float LobbyQueryRate = 1.1f;

        static Lobby Lobby
        {
            get => _lobby;
            set
            {
                _lobby = value;
                _lobbyUpdateTimer = LobbyUpdateTimerMax;
            }
        }
        static Lobby _lobby;

        static float? _heartbeatTimer; // heartbeat time is null when heartbeat operation is in progress
        static float? _lobbyUpdateTimer;

        static float _lobbyQueryTimer;
        static Action<List<(string lobbyId, string lobbyName, int playerCount, int playerMax)>> _pendingLobbyQueryCallback;

        /// <summary>
        /// prevents multiple signal calling.
        /// </summary>
        static int _lastUpdateCallHash = int.MinValue;

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
                UpdatePlayerName(GameLogicData.PlayerName);

            if (Lobby == null)
                return;

            HandleLobbyCallForUpdates();

            if (IsHost)
                HandleLobbyHeartbeat();
        }

        // lobbies are automatically turn inactive if the lobby does not receive any data
        // for 30 seconds
        // inactive means other players cannot find it but the players that are inside can still normally operate
        // todo: disable heartbeat after reached max player
        // todo: and enable it again when below max
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

                Lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
                _heartbeatTimer = HeartbeatRate;

                SignalProcessor.SendSignal(new LobbyChangedSignal(Lobby.Name, GetPlayers()));
                Debug.Log("Created lobby " + Lobby.Name + " " + Lobby.MaxPlayers);
                return true;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                return true;
            }
        }

        internal static void RequestGetLobbies(Action<List<(string lobbyId, string lobbyName, int playerCount, int playerMax)>> callback)
        {
            Assert.IsNotNull(callback, "callback function cannot be null.");

            _pendingLobbyQueryCallback = callback;
        }

        static async void ExecuteLobbyQueryCallback()
        {
            _lobbyQueryTimer = LobbyQueryRate;
            List<(string lobbyId, string lobbyName, int playerCount, int playerMax)> lobbies = await QueryLobbies();
            _pendingLobbyQueryCallback.Invoke(lobbies);
            _pendingLobbyQueryCallback = null;
        }

        /// <summary>
        /// Rate limit for lobby querying is 1 query per second.
        /// Calling this method more often will result in an error.
        /// </summary>
        static async Task<List<(string lobbyId, string lobbyName, int playerCount, int playerMax)>> QueryLobbies()
        {
            List<(string, string, int, int)> list = new();
            try
            {
                var options = new QueryLobbiesOptions {Count = 25};

                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);
                Debug.Log("Lobbies found: " + queryResponse.Results.Count);

                list.AddRange(queryResponse.Results.Select(lobby => (lobby.Id, lobby.Name, lobby.Players.Count, lobby.MaxPlayers)));
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

            return list;
        }

        internal static async void JoinLobbyById(string lobbyId, Action<string, List<(string playerName, string playerId, bool isHost)>> callback)
        {
            try
            {
                var options = new JoinLobbyByIdOptions
                {
                    Player = new Player
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            {"playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, GameLogicData.PlayerName)}
                        }
                    }
                };

                Lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, options);
                Debug.Log("Joined lobby");
                PrintPlayers(Lobby);
                callback(Lobby.Name, GetPlayers());
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        internal static async void JoinLobbyByCode(string lobbyCode)
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

                Lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
                Debug.Log("Joined lobby");
                SignalProcessor.SendSignal(new LobbyChangedSignal(Lobby.Name, GetPlayers()));
                PrintPlayers(Lobby);
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

        /// <summary>
        /// If the instance hosted a lobby, the lobby will be deleted.
        /// </summary>
        internal static void SignOut()
        {
            // delete lobby if any and only one player is present
            if (Lobby != null)
                LobbyService.Instance.DeleteLobbyAsync(Lobby.Id);

            AuthenticationService.Instance.SignOut(true);
        }

        static void PrintPlayers(Lobby lobby)
        {
            foreach (Player player in lobby.Players)
                Debug.Log($"Player info, id: {player.Id}, name: {player.Data["playerName"].Value}");
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
                Lobby = await Lobbies.Instance.UpdateLobbyAsync(
                    Lobby.Id, new UpdateLobbyOptions {Data = new Dictionary<string, DataObject>
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
                Lobby = await LobbyService.Instance.UpdatePlayerAsync(
                    Lobby.Id,
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
                ILobbyService lobby = LobbyService.Instance;
                string playerId = AuthenticationService.Instance.PlayerId;

                // if host is the last player, delete lobby 
                if (IsHost)
                {
                    await lobby.RemovePlayerAsync(Lobby.Id, playerId);

                    if (Lobby.Players.Count > 1)
                        await lobby.DeleteLobbyAsync(Lobby.Id);
                }
                else
                {
                    await lobby.RemovePlayerAsync(Lobby.Id, playerId);
                }
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
                await LobbyService.Instance.RemovePlayerAsync(Lobby.Id, playerId);
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
                Lobby = await Lobbies.Instance.UpdateLobbyAsync(Lobby.Id, new UpdateLobbyOptions {HostId = Lobby.Players[1].Id});
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }

        /// <summary>
        /// Lobbies becomes inactive after 30s. In order to prevent that we have to ping it every <see cref="HeartbeatRate"/>.
        /// Heartbeat is disabled when player count reach the maximum.
        /// It is enabled again when it is goes down below maximum.
        /// </summary>
        static async void HandleLobbyHeartbeat()
        {
            Assert.IsNotNull(Lobby, $"This method should not be called if {nameof(Lobby)} variable is null");

            // null means operation is in progress
            if (_heartbeatTimer == null)
                return;

            _heartbeatTimer -= Time.deltaTime;
            //Debug.Log(_heartbeatTimer);
            if (_heartbeatTimer < 0f)
            {
                _heartbeatTimer = null;
                await RestoreSessionIfNeeded();

                Debug.Log("is signed in: " + AuthenticationService.Instance.IsSignedIn);
                Debug.Log("host id: " + Lobby.Id);
                Debug.Log("player id: " + AuthenticationService.Instance.PlayerId);
                await LobbyService.Instance.SendHeartbeatPingAsync(Lobby.Id);
                _heartbeatTimer = HeartbeatRate;
            }
        }

        /// <summary>
        /// Player's session may occasionally expire.
        /// This function should be called each time we call the lobby API to avoid 403 Unauthorized exception.
        /// </summary>
        static async Task RestoreSessionIfNeeded()
        {
            if (AuthenticationService.Instance.IsSignedIn)
                return;

            Debug.Log("Session expired. Restoration in progress.");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        /// <summary>
        /// Every <see cref="LobbyUpdateTimerMax"/> it updates <see cref="Lobby"/> reference with a new instance.
        /// It is called both on client and the host.
        /// </summary>
        static async void HandleLobbyCallForUpdates()
        {
            Assert.IsNotNull(Lobby, $"This method should not be called if {nameof(Lobby)} variable is null");

            // null means operation is in progress
            if (_lobbyUpdateTimer == null)
                return;

            _lobbyUpdateTimer -= Time.deltaTime;
            if (_lobbyUpdateTimer >= 0f)
                return;

            _lobbyUpdateTimer = null;

            try
            {
                await RestoreSessionIfNeeded();
                Lobby = await LobbyService.Instance.GetLobbyAsync(Lobby.Id);
                _lobbyUpdateTimer = LobbyUpdateTimerMax;

                // calculate hash
                List<(string playerName, string PlayerId, bool isHost)> players = GetPlayers();
                Debug.Log("player count: " + Lobby.Players.Count);
                int hashCode = CalculateHash(Lobby.Name, players);
                Debug.Log("hash code: " + hashCode);

                // send signal is if has changed
                if (hashCode != _lastUpdateCallHash)
                {
                    SignalProcessor.SendSignal(new LobbyChangedSignal(Lobby.Name, players));
                    _lastUpdateCallHash = hashCode;
                }
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }

        static List<(string playerName, string playerId, bool isHost)> GetPlayers()
        {
            var players = new List<(string playerName, string playerId, bool isHost)>();
            foreach (Player player in Lobby.Players)
                // todo: name is an additional data and has to be transferred differently
                players.Add((GameLogicData.PlayerName, player.Id, player.Id == Lobby.HostId));

            return players;
        }

        static int CalculateHash(string lobbyName, List<(string playerName, string playerId, bool isHost)> players)
        {
            int hashCode = lobbyName.GetHashCode();

            foreach ((string playerName, string playerId, bool isHost) in players)
            {
                // todo: in real life this probably cannot be null
                if (playerName != null)
                    hashCode += playerName.GetHashCode();

                hashCode += playerId.GetHashCode() + isHost.GetHashCode();
            }

            return hashCode;
        }
    }
}