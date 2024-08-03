#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using System.Text;
using System;
using Core;
using Core.Systems;
using UI.Config;
using UI.Systems;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class ServerListPopup : AbstractPopup
    {
        [SerializeField]
        Transform _serverContainer;

        [SerializeField]
        Button _joinButton;

        [SerializeField]
        Button _createServerButton;

        [SerializeField]
        TMP_InputField _ipInputField;

        [SerializeField]
        TMP_InputField _portInputField;

        const string KeyId = "89302e22-e73b-4890-80fd-04e29f27a721";
        const string KeySecret = "cQb6Cj1nV6QyIsflmwVxt-ZTLtEpa8_P";
        const string ProjectId = "f99e7a47-7455-4b68-a7fd-0b4c6e99c755";
        const string EnvironmentId = "3a7f6955-55a9-4574-94b0-8c4ef77f8666";
        const string FleetId = "89aa5df1-6886-45e4-b808-a1b67563367a";

        static readonly UIConfig _config;

        public ServerListPopup(PopupType type)
            : base(type) { }

        void Awake()
        {
            _joinButton.onClick.AddListener(() =>
            {
                string ipv4Address = _ipInputField.text;
                ushort port = ushort.Parse(_portInputField.text);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port);

                // todo: temporary disabled
                //KitchenGameMultiplayer.Instance.StartClient();

                // todo: RADEK's start client start here
                CoreData.IsMultiplayer = true;
                CoreData.CurrentLevel = Level.HubLocation;

                // this will start the netcode client
                GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CoreData.CurrentLevel});
            });

            _createServerButton.onClick.AddListener(() =>
            {
                byte[] keyByteArray = Encoding.UTF8.GetBytes(KeyId + ":" + KeySecret);
                string keyBase64 = Convert.ToBase64String(keyByteArray);
                string url = $"https://services.api.unity.com/auth/v1/token-exchange?projectId={ProjectId}&environmentId={EnvironmentId}";

                string jsonRequestBody = JsonUtility.ToJson(new TokenExchangeRequest
                {
                    Scopes = new[] {"multiplay.allocations.create", "multiplay.allocations.list"},
                });

                WebRequestSystem.PostJson(url,
                                          unityWebRequest => unityWebRequest.SetRequestHeader("Authorization", "Basic " + keyBase64),
                                          jsonRequestBody,
                                          error => Debug.Log("Error: " + error),
                                          ParseServerPost);
            });

            // todo: destroy children
            // todo: intantiate new

            /*serverTemplate.gameObject.SetActive(false);
            foreach (Transform child in serverContainer)
            {
                if (child == serverTemplate)
                    continue;

                Destroy(child.gameObject);
            }*/
        }

        internal static void TEST_TROLL()
        {
            byte[] keyByteArray = Encoding.UTF8.GetBytes(KeyId + ":" + KeySecret);
            string keyBase64 = Convert.ToBase64String(keyByteArray);

            string url = $"https://services.api.unity.com/multiplay/servers/v1/projects/{ProjectId}/environments/{EnvironmentId}/servers";

            WebRequestSystem.Get(url,
                                 unityWebRequest => unityWebRequest.SetRequestHeader("Authorization", "Basic " + keyBase64),
                                 error => Debug.Log("Error: " + error),
                                 ParseGetServersJson);
        }

        static void ParseServerPost(string json)
        {
            Debug.Log("Success: " + json);
            var tokenExchangeResponse = JsonUtility.FromJson<TokenExchangeResponse>(json);

            string url
                = $"https://multiplay.services.api.unity.com/v1/allocations/projects/{ProjectId}/environments/{EnvironmentId}/fleets/{FleetId}/allocations";

            WebRequestSystem.PostJson(
                url,
                unityWebRequest => unityWebRequest.SetRequestHeader("Authorization", "Bearer " + tokenExchangeResponse.AccessToken),
                JsonUtility.ToJson(new QueueAllocationRequest
                {
                    AllocationId = "AAAAAAAAAAAAA",
                    BuildConfigurationId = 0,
                    RegionId = "AAAAAAAAAAAAAAAAA"
                }), error => Debug.Log("Error: " + error), json => Debug.Log("Success: " + json));
        }

#if !DEDICATED_SERVER
        void Start()
        {
            byte[] keyByteArray = Encoding.UTF8.GetBytes(KeyId + ":" + KeySecret);
            string keyBase64 = Convert.ToBase64String(keyByteArray);

            string url = $"https://services.api.unity.com/multiplay/servers/v1/projects/{ProjectId}/environments/{EnvironmentId}/servers";

            // requires "Game Server Hosting API Viewer" permission
            WebRequestSystem.Get(url,
                                 unityWebRequest => unityWebRequest.SetRequestHeader("Authorization", "Basic " + keyBase64),
                                 error => Debug.Log("Error: " + error),
                                 ParseGetServersJson);
        }

        static void ParseGetServersJson(string json)
        {
            Debug.Log("Success: " + json);
            var listServers = JsonUtility.FromJson<ListServers>("{\"serverList\":" + json + "}");

            if (listServers.ServerList == null)
                return;
            
            // todo: create servers
            foreach (Server server in listServers.ServerList)
                //Debug.Log(server.ip + " : " + server.port + " " + server.deleted + " " + server.status);
                if (server.Status == ServerStatus.Online.ToString() || server.Status == ServerStatus.Allocated.ToString())
                {
                    // Server is Online!
                    //Transform serverTransform = Instantiate(serverTemplate, serverContainer);
                    //serverTransform.gameObject.SetActive(true);

                    Debug.Log("Destroy OLD create NEW");

                    // todo: temporary commented out
                    //serverTransform.GetComponent<ServerBrowserSingleUI>().SetServer(server.ip, (ushort)server.port);
                }
        }
#endif

        public class TokenExchangeResponse
        {
            public string AccessToken;
        }

        [Serializable]
        public class TokenExchangeRequest
        {
            public string[] Scopes;
        }

        [Serializable]
        public class QueueAllocationRequest
        {
            public string AllocationId;
            public int BuildConfigurationId;
            public string Payload;
            public string RegionId;
            public bool Restart;
        }

        [Serializable]
        public class ListServers
        {
            public Server[]? ServerList;
        }

        [Serializable]
        public class Server
        {
            public int BuildConfigurationID;
            public string BuildConfigurationName;
            public string BuildName;
            public bool Deleted;
            public string FleetID;
            public string FleetName;
            public string HardwareType;
            public int Id;
            public string Ip;
            public int LocationID;
            public string LocationName;
            public int MachineID;
            public int Port;
            public string Status;
        }
    }
}