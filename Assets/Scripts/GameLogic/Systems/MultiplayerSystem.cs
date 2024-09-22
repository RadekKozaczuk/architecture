#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Unity.Netcode.Transports.UTP;
using Unity.Services.Multiplay;
using System.Collections;
using Unity.Services.Core;
using Unity.Netcode;
using UnityEngine;
using Shared;
using System;

namespace GameLogic.Systems
{
    public static class MultiplayerSystem
    {
        internal static string IpAddress;
        internal static string Port;

        static IServerQueryHandler? _serverQueryHandler;

        internal static async void StartServer()
        {
            await UnityServices.InitializeAsync();

            ServerConfig serverConfig = MultiplayService.Instance.ServerConfig;
            _serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(2, "n/a", "n/a", "0", "n/a");

            // tells if the server is up and running
            if (serverConfig.AllocationId == string.Empty)
                return;

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("0.0.0.0", serverConfig.Port, "0.0.0.0");
            if (!NetworkManager.Singleton.StartServer())
            {
                Debug.LogError("Failed to start server");
                throw new Exception("Failed to start server");
            }

            MultiplayEventCallbacks callbacks = new MultiplayEventCallbacks();
            callbacks.Allocate += async _ =>
            {
                // inform the matchmaker that the server is ready to receive players
                await MultiplayService.Instance.ReadyServerForPlayersAsync();
                // start updating server invoked by coroutine
                StaticCoroutine.StartStaticCoroutine(ServerUpdateCoroutine());
            };
            callbacks.Deallocate += _ =>
            {
                MultiplayService.Instance.UnreadyServerAsync();
            };

            await MultiplayService.Instance.SubscribeToServerEventsAsync(callbacks);
        }

        internal static void JoinServer() => NetworkManager.Singleton.StartClient();

        static IEnumerator ServerUpdateCoroutine()
        {
            while (true)
            {
                // process when server is still allocated
                if (_serverQueryHandler == null || !Application.isPlaying)
                    yield break;

                if (NetworkManager.Singleton.IsServer)
                    _serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsIds.Count;

                _serverQueryHandler.UpdateServerCheck();
                yield return null;
            }
        }
    }
}