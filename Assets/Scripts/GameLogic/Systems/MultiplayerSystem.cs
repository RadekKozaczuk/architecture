﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using System.Collections;
using System.Threading.Tasks;
using Shared;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Unity.Services.Multiplay;
using Unity.Services.Core;

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
            if (serverConfig.AllocationId != string.Empty)
            {
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("0,0,0,0", serverConfig.Port, "0,0,0,0");
                NetworkManager.Singleton.StartServer();

                // inform the matchmaker that the server is ready to receive players
                await MultiplayService.Instance.ReadyServerForPlayersAsync();

                // start updating server invoked by coroutine
                StaticCoroutine.StartStaticCoroutine(ServerUpdateCoroutine());
            }
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