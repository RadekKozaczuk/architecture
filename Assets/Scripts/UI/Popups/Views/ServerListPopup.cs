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
using UI.Systems;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class ServerListPopup : AbstractPopup
    {
        [SerializeField]
        Transform serverContainer;
        [SerializeField]
        Transform serverTemplate;
        [SerializeField]
        Button joinIPButton;
        [SerializeField]
        Button createServerButton;
        [SerializeField]
        TMP_InputField ipInputField;
        [SerializeField]
        TMP_InputField portInputField;

        public ServerListPopup(PopupType type)
            : base(type) { }

        void Awake()
        {
            joinIPButton.onClick.AddListener(() =>
            {
                string ipv4Address = ipInputField.text;
                ushort port = ushort.Parse(portInputField.text);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port);

                // todo: temporary disabled
                //KitchenGameMultiplayer.Instance.StartClient();

                // todo: RADEK's start client start here
                CoreData.IsMultiplayer = true;
                CoreData.CurrentLevel = Level.HubLocation;

                // this will start the netcode client
                GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CoreData.CurrentLevel});
            });

            createServerButton.onClick.AddListener(() =>
            {
                string keyId = "AAAAAAAAAAAAA";
                string keySecret = "AAAAAAAAAAAA";
                byte[] keyByteArray = Encoding.UTF8.GetBytes(keyId + ":" + keySecret);
                string keyBase64 = Convert.ToBase64String(keyByteArray);

                string projectId = "AAAAAAAAAAAAAAAAA";
                string environmentId = "AAAAAAAAAAAAAAAAAAAAA";
                string url = $"https://services.api.unity.com/auth/v1/token-exchange?projectId={projectId}&environmentId={environmentId}";

                string jsonRequestBody
                    = JsonUtility.ToJson(new TokenExchangeRequest {scopes = new[] {"multiplay.allocations.create", "multiplay.allocations.list"},});

                WebRequestSystem.PostJson(url, unityWebRequest => unityWebRequest.SetRequestHeader("Authorization", "Basic " + keyBase64), jsonRequestBody,
                                     error => Debug.Log("Error: " + error), json =>
                                     {
                                         Debug.Log("Success: " + json);
                                         var tokenExchangeResponse = JsonUtility.FromJson<TokenExchangeResponse>(json);

                                         string fleetId = "AAAAAAAAAAAAAAAA";
                                         string url
                                             = $"https://multiplay.services.api.unity.com/v1/allocations/projects/{projectId}/environments/{environmentId}/fleets/{fleetId}/allocations";

                                         WebRequestSystem.PostJson(
                                             url,
                                             unityWebRequest =>
                                                 unityWebRequest.SetRequestHeader("Authorization", "Bearer " + tokenExchangeResponse.accessToken),
                                             JsonUtility.ToJson(new QueueAllocationRequest
                                             {
                                                 allocationId = "AAAAAAAAAAAAA",
                                                 buildConfigurationId = 0,
                                                 regionId = "AAAAAAAAAAAAAAAAA"
                                             }), error => Debug.Log("Error: " + error), json => Debug.Log("Success: " + json));
                                     });
            });

            serverTemplate.gameObject.SetActive(false);
            foreach (Transform child in serverContainer)
            {
                if (child == serverTemplate)
                    continue;

                Destroy(child.gameObject);
            }
        }

#if !DEDICATED_SERVER
        void Start()
        {
            string keyId = "AAAAAAAAAAAA";
            string keySecret = "AAAAAAAAAAAA";
            byte[] keyByteArray = Encoding.UTF8.GetBytes(keyId + ":" + keySecret);
            string keyBase64 = Convert.ToBase64String(keyByteArray);

            string projectId = "AAAAAAAAAAAAAAA";
            string environmentId = "AAAAAAAAAAAAAAAAAAAAA";
            string url = $"https://services.api.unity.com/multiplay/servers/v1/projects/{projectId}/environments/{environmentId}/servers";

            WebRequestSystem.Get(url, unityWebRequest => unityWebRequest.SetRequestHeader("Authorization", "Basic " + keyBase64),
                            error => Debug.Log("Error: " + error),
                            json =>
                            {
                                Debug.Log("Success: " + json);
                                var listServers = JsonUtility.FromJson<ListServers>("{\"serverList\":" + json + "}");
                                foreach (Server server in listServers.serverList)
                                    //Debug.Log(server.ip + " : " + server.port + " " + server.deleted + " " + server.status);
                                    if (server.status == ServerStatus.Online.ToString() || server.status == ServerStatus.Allocated.ToString())
                                    {
                                        // Server is Online!
                                        Transform serverTransform = Instantiate(serverTemplate, serverContainer);
                                        serverTransform.gameObject.SetActive(true);

                                        // todo: temporary commented out
                                        //serverTransform.GetComponent<ServerBrowserSingleUI>().SetServer(server.ip, (ushort)server.port);
                                    }
                            });
        }
#endif

        public class TokenExchangeResponse
        {
            public string accessToken;
        }

        [Serializable]
        public class TokenExchangeRequest
        {
            public string[] scopes;
        }

        [Serializable]
        public class QueueAllocationRequest
        {
            public string allocationId;
            public int buildConfigurationId;
            public string payload;
            public string regionId;
            public bool restart;
        }

        [Serializable]
        public class ListServers
        {
            public Server[] serverList;
        }

        [Serializable]
        public class Server
        {
            public int buildConfigurationID;
            public string buildConfigurationName;
            public string buildName;
            public bool deleted;
            public string fleetID;
            public string fleetName;
            public string hardwareType;
            public int id;
            public string ip;
            public int locationID;
            public string locationName;
            public int machineID;
            public int port;
            public string status;
        }
    }
}