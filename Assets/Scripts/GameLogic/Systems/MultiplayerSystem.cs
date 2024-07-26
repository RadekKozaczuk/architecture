#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Threading.Tasks;
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

        // should only be run if MachineRole = DedicatedServer
        internal static async void CustomStart()
        {
            // todo: this hinda makes no sense as in our case it will always be dedicated server
            //if (Application.platform == RuntimePlatform.LinuxServer)
            //{
                // on linux server framerate is not initially set
                // todo: probably to be deleted as we set it in Boot
                Application.targetFrameRate = 30;

                await UnityServices.InitializeAsync();

                ServerConfig serverConfig = MultiplayService.Instance.ServerConfig;

                _serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(10, "MyServer", "MyGameType", "0", "TestMap");

                // tells if the server is up and running
                if (serverConfig.AllocationId != string.Empty)
                {
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("0,0,0,0", serverConfig.Port, "0,0,0,0");
                    NetworkManager.Singleton.StartServer();

                    // inform the matchmaker that the server is ready to receive players
                    await MultiplayService.Instance.ReadyServerForPlayersAsync();
                }
            //}
        }

        // should only be run if MachineRole = DedicatedServer
        internal static async void CustomUpdate()
        {
            // todo: this hinda makes no sense as in our case it will always be dedicated server
            //if (Application.platform == RuntimePlatform.LinuxServer)
            //{
            if (_serverQueryHandler != null)
            {
                _serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsIds.Count;
                _serverQueryHandler.UpdateServerCheck();
                await Task.Delay(100);
            }
            //}
        }

        internal static void JoinServer()
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(IpAddress, ushort.Parse(Port));
            NetworkManager.Singleton.StartClient();
        }
    }
}