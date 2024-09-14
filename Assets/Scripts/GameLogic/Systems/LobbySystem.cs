#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core;
using Core.Dtos;
using Core.Enums;
using Core.Systems;
using Shared;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameLogic.Systems
{
	/// <summary>
	/// This is system is responsible for creating a multiplayer lobby as well as for handling voice chat.
	/// </summary>
	static class LobbySystem
    {
        /// <summary>
        /// Indicates if the system is currently processing a call. Consecutive calls are not allowed.
        /// </summary>
        internal static bool RequestInProgress
        {
            get;
            private set;
        }

        static bool IsHost => AuthenticationService.Instance.PlayerId == Lobby!.HostId;

        /// <summary>
        /// In seconds. Heartbeat is limited to 5 requests per 30 seconds.
        /// </summary>
        const float HeartbeatRate = 25f;
        const float LobbyUpdateTimerMax = 1.1f;
        /// <summary>
        /// Maximum allowed lobby query rate by Unity is 1 query per seconds. Executing queries faster will result in an error.
        /// For safety, we allow execution slightly slower than this.
        /// </summary>
        const float LobbyQueryRate = 1.5f;

        /// <summary>
        /// Currently joined lobby.
        /// </summary>
        static Lobby? Lobby
        {
            get => _lobby;
            set
            {
                _lobby = value;
                _lobbyUpdateTimer = LobbyUpdateTimerMax;
            }
        }
        static Lobby? _lobby;

        static float? _heartbeatTimer; // heartbeat time is null when heartbeat operation is in progress
        static float? _lobbyUpdateTimer;

        static float _lobbyQueryTimer;
        static string _relayCode;

        /// <summary>
        /// Prevents multiple signal calling.
        /// </summary>
        static int _lastUpdateCallHash = int.MinValue;

        /// <summary>
        /// Indicates that the player changed its name.
        /// </summary>
        static bool _lobbyIsDirty;

        static bool _gameStarted;

        internal static void CustomUpdate()
        {
            if (_lobbyQueryTimer > 0)
                _lobbyQueryTimer -= Time.deltaTime;

            if (_lobbyIsDirty)
                UpdatePlayerName(CoreData.PlayerName);

            if (Lobby == null)
                return;

            HandleLobbyCallForUpdates();

            if (IsHost)
                HandleLobbyHeartbeat();
        }

        internal static async Task<Lobby> GetLobbyAsync(string lobbyId)
        {
            RequestInProgress = true;

            try
            {
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
                RequestInProgress = false;
                return lobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

            RequestInProgress = false;
            return null!;
        }

        internal static async Task<List<string>> GetJoinedLobbiesAsync()
        {
            RequestInProgress = true;

            try
            {
                List<string> lobbies = await LobbyService.Instance.GetJoinedLobbiesAsync();
                RequestInProgress = false;
                return lobbies;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

            RequestInProgress = false;
            return null!;
        }

        // lobbies are automatically turn inactive if the lobby does not receive any data for 30 seconds
        // inactive means other players cannot find it but the players that are inside can still normally operate
        /// <summary>
        /// If the lobby was successfully created it returns true, the first player's id and lobby code, false, null and null otherwise.
        /// </summary>
        internal static async Task<(bool success, string playerId, string lobbyCode)> CreateLobby(string lobbyName, int maxPlayers)
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
                            {Constants.PlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, CoreData.PlayerName)}
                        }
                    }
                };

                Lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

                _heartbeatTimer = HeartbeatRate;
                VoiceChatSystem.JoinChannel(_lobby!.Name, true, false);
                return (true, Lobby.Players[0].Id, Lobby.LobbyCode);
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
                return (false, null!, null!);
            }
            catch (RelayServiceException e)
            {
                MyDebug.Log(e.ToString());
                return (false, null!, null!);
            }
        }

        internal static async void JoinLobbyById(string lobbyId,
            Action<string, string, List<(string playerName, string playerId, bool isHost)>> callback)
        {
            try
            {
                var options = new JoinLobbyByIdOptions
                {
                    Player = new Player
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            {Constants.PlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, CoreData.PlayerName)}
                        }
                    }
                };

                Lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, options);
                callback(Lobby.Name, Lobby.LobbyCode, GetPlayers());
                VoiceChatSystem.JoinChannel(_lobby!.Name, true, false);
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }

        internal static async void JoinLobbyByCode(string lobbyCode,
            Action<string, string, List<(string playerName, string playerId, bool isHost)>> callback)
        {
            try
            {
                var options = new JoinLobbyByCodeOptions
                {
                    Player = new Player
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            {Constants.PlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, CoreData.PlayerName)}
                        }
                    }
                };

				Lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
				callback(Lobby.Name, Lobby.LobbyCode, GetPlayers());
                VoiceChatSystem.JoinChannel(_lobby!.Name, true, false);
			}
			catch (LobbyServiceException e)
			{
				MyDebug.Log(e.ToString());
				callback(null!, null!, null!);
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
                VoiceChatSystem.JoinChannel(_lobby!.Name, true, false);
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }

        internal static async void RejoinToLobby(string lobbyId,
            Action<string, string, List<(string playerName, string playerId, bool isHost)>> callback)
        {
            try
            {
                Lobby = await Lobbies.Instance.ReconnectToLobbyAsync(lobbyId);
                callback(Lobby.Name, Lobby.LobbyCode, GetPlayers());
                VoiceChatSystem.JoinChannel(_lobby!.Name, true, false);
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
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

        internal static async void LeaveLobby()
        {
            try
            {
                ILobbyService lobby = LobbyService.Instance;
                string playerId = AuthenticationService.Instance.PlayerId;

                // if host is the last player, delete lobby 
                if (IsHost)
                {
                    if (Lobby!.Players.Count == 1)
                    {
                        await lobby.DeleteLobbyAsync(Lobby.Id);
                        Lobby = null;
                    }
                    else
                    {
                        GiveHost(GetRandomPlayerIdWithoutLocalPlayer());
                        await lobby.RemovePlayerAsync(Lobby.Id, playerId);
                    }
                }
                else
                    await lobby.RemovePlayerAsync(Lobby!.Id, playerId);

                VoiceChatSystem.LeaveChannel();
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }

        internal static async void RemovePlayerFromLobby(string lobbyId)
        {
            Assert.IsNotNull(lobbyId, $"Parameter {nameof(lobbyId)} cannot be null.");
            try
            {
                string playerId = AuthenticationService.Instance.PlayerId;
                await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }

        internal static async Task StartGame_Host()
        {
            // Important: Once the allocation is created, you have ten seconds to BIND
            _gameStarted = true;
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(Lobby!.MaxPlayers - 1);
            _relayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            var relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

            Lobby = await Lobbies.Instance.UpdateLobbyAsync(Lobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    // member is visible only for people inside the lobby
                    {Constants.RelayCode, new DataObject(DataObject.VisibilityOptions.Member, _relayCode)}
                }
            });

            CoreData.PlayerCount = Lobby.Players.Count;
            GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CoreData.CurrentLevel});
        }

        /// <summary>
        /// Only host can kick players. Host cannot kick himself.
        /// </summary>
        internal static async void KickPlayer(string playerId)
        {
            Assert.IsNotNull(playerId, $"Parameter {nameof(playerId)} cannot be null.");

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(Lobby!.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }

        /// <summary>
        /// Gives lobby host role to one of the players.
        /// </summary>
        internal static async void GiveHost(string playerId)
        {
            try
            {
                Lobby = await Lobbies.Instance.UpdateLobbyAsync(Lobby!.Id, new UpdateLobbyOptions {HostId = playerId});
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }

        /// <summary>
        /// Rate limit for lobby querying is 1 query per second.
        /// Calling this method more often will result in an error.
        /// </summary>
        internal static async Task<LobbyDto[]> GetLobbiesAsync()
        {
            RequestInProgress = true;

            if (_lobbyQueryTimer > 0)
                await Task.Delay((int)(_lobbyQueryTimer * 1000));

            try
            {
                var options = new QueryLobbiesOptions {Count = 25};

                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);

                var array = new LobbyDto[queryResponse.Results.Count];
                for (int i = 0; i < queryResponse.Results.Count; i++)
                {
                    Lobby lobby = queryResponse.Results[i];
                    array[i] = new LobbyDto(lobby.Id, lobby.Name, lobby.Players.Count, lobby.MaxPlayers);
                }

                _lobbyQueryTimer = LobbyQueryRate;
                RequestInProgress = false;
                return array;
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }

            RequestInProgress = false;
            return null!;
        }

        static async void UpdatePlayerName(string playerName)
        {
            try
            {
                // we should make isDirty pattern and call update when update happened
                Lobby = await LobbyService.Instance.UpdatePlayerAsync(Lobby!.Id, AuthenticationService.Instance.PlayerId,
                      new UpdatePlayerOptions
                      {
                          Data = new Dictionary<string, PlayerDataObject>
                          {
                              {
                                  Constants.PlayerName,
                                  new PlayerDataObject(
                                      PlayerDataObject.VisibilityOptions.Member, playerName)
                              }
                          }
                      });
            }
            catch (LobbyServiceException e)
            {
                MyDebug.Log(e.ToString());
            }
        }

        static async void StartGame_Client()
        {
            CoreData.IsMultiplayer = true;
            CoreData.CurrentLevel = Level.HubLocation;

            try
            {
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(_relayCode);
                var serverData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
            }
            catch (Exception e)
            {
                MyDebug.Log(e.ToString());
            }

            GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CoreData.CurrentLevel});
        }

        /// <summary>
        /// Lobbies becomes inactive after 30s. In order to prevent that we have to ping it every <see cref="HeartbeatRate" />.
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
            if (_heartbeatTimer < 0f)
            {
                _heartbeatTimer = null;
                await RestoreSessionIfNeeded();
                await LobbyService.Instance.SendHeartbeatPingAsync(Lobby!.Id);
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

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        /// <summary>
        /// Every <see cref="LobbyUpdateTimerMax" /> it updates <see cref="Lobby" /> reference with a new instance.
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
                try
                {
                    Lobby = await LobbyService.Instance.GetLobbyAsync(Lobby!.Id);
                }
                catch (LobbyServiceException e)
                {
                    MyDebug.Log(e.ToString());
                    return;
                }

                if (GameStateSystem.CurrentState == GameState.MainMenu
                    && Lobby.Data != null
                    && Lobby.Data.TryGetValue(Constants.RelayCode, out DataObject relayCode))
                {
                    _relayCode = relayCode.Value;
                    StartGame_Client();
                    Lobby = null; // we don't want to update it anymore
                    return;
                }

                _lobbyUpdateTimer = LobbyUpdateTimerMax;

                // calculate hash
                int hashCode = CalculateHash();

                // send signal if it has changed
                if (hashCode != _lastUpdateCallHash && !_gameStarted)
                {
                    Signals.LobbyChanged(Lobby.Name, Lobby.LobbyCode, GetPlayers());
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
            foreach (Player player in Lobby!.Players)
            {
                string playerName = player.Data[Constants.PlayerName].Value;
                players.Add((playerName, player.Id, player.Id == Lobby.HostId));
            }

            return players;
        }

        static string GetRandomPlayerIdWithoutLocalPlayer()
        {
            List<(string playerName, string playerId, bool isHost)> players = GetPlayers();
            foreach ((string _, string playerId, bool _) in players)
                if (playerId != AuthenticationService.Instance.PlayerId)
                    return playerId;

            return null!;
        }

        /// <summary>
        /// Hash function is only used internally to reduce the amount of signals sent.
        /// </summary>
        static int CalculateHash()
        {
            int hashCode = _lobby!.Name.GetHashCode();

            foreach (Player player in _lobby.Players)
                if (IsHost)
                    hashCode += 2 * player.Data[Constants.PlayerName].Value.GetHashCode();
                else
                    hashCode += player.Data[Constants.PlayerName].Value.GetHashCode();

            return hashCode;
        }
    }
}