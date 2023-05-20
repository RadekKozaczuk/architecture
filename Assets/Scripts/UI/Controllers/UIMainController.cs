using System.Collections.Generic;
using Common.Signals;
using ControlFlow.Interfaces;
using ControlFlow.SignalProcessing;
using JetBrains.Annotations;
using Shared;
using UI.Systems;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Scripting;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Shared.DebugCommands;
#endif

namespace UI.Controllers
{
    /// <summary>
    /// Main controller serves 3 distinct roles:
    /// 1) It allows you to order the signal execution order. So instead of reacting separately in two different controllers you can react in main controller
    /// and call adequate methods.
    /// 2) Serves as a 'default' controller. When you don't know where to put some logic or the logic is too small for its own controller you can put it into
    /// the main controller.
    /// 3) Reduces the size of the view model. We could move all (late/fixed)update calls to view model but as the application grows it would lead to view
    /// model doing to much stuff.
    /// For better code readability all controllers meant to interact with this controller should implement
    /// <see cref="ICustomLateUpdate" /> interface.
    /// </summary>
    [ReactOnSignals]
    [UsedImplicitly]
    class UIMainController : ICustomFixedUpdate, ICustomUpdate, ICustomLateUpdate
    {
        static bool _uiSceneLoaded;

        Lobby _hostLobby;
        float _heartbeatTimer;

        [Preserve]
        UIMainController() { }

        public void CustomUpdate()
        {
            if (!_uiSceneLoaded)
                return;

            InputSystem.CustomUpdate();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            DebugCommandSystem.CustomUpdate();
#endif

            if (_hostLobby == null)
                return;

            HandleLobbyHeartbeat();
        }

        public void CustomFixedUpdate() { }

        public void CustomLateUpdate() { }

        internal static void OnUISceneLoaded() => _uiSceneLoaded = true;

        // lobbies are automatically turn inactive if the lobby does not recieve any data
        // for 30 seconds
        async void CreateLobby()
        {
            try
            {
                string lobbyName = "MyLobby";
                int maxPlayers = 4;
                var options = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Player = GetPlayer()
                };

                _hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

                Debug.Log("Created lobby " + _hostLobby.Name + " " + _hostLobby.MaxPlayers);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        async void ListLobbies()
        {
            try
            {
                var options = new QueryLobbiesOptions {Count = 25};

                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);
                Debug.Log("Lobbies found: " + queryResponse.Results.Count);

                foreach (Lobby lobby in queryResponse.Results)
                {
                    Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        async void JoinLobby(string lobbyCode)
        {
            try
            {
                var options = new JoinLobbyByCodeOptions
                {
                    Player = GetPlayer()
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

        async void QuickJoinLobby()
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

        static Player GetPlayer() =>
            new()
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {"playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "name1")}
                }
            };

        /// <summary>
        /// Lobbies becomes inactive after 30s. In order to prevent that we have to ping it every 20s or so.
        /// </summary>
        void HandleLobbyHeartbeat()
        {
            Assert.IsNotNull(_hostLobby, $"This method should not be called if {nameof(_hostLobby)} variable is null");

            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer < 0f)
            {
                _heartbeatTimer = 20;
                LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
            }
        }

        [React]
        [Preserve]
        void OnInventoryChangedSignal(InventoryChangedSignal _) { }
    }
}