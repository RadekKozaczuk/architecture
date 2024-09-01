#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Dtos;
using Shared;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace GameLogic.Systems
{
    static class WebRequestSystem
    {
        [Serializable]
        public class TokenExchangeRequest
        {
#pragma warning disable RAD201
            public string[] scopes;
#pragma warning restore RAD201
        }

        [Serializable]
        public class AllocationRequest
        {
#pragma warning disable RAD201
            public string allocationId;
            public int buildConfigurationId;
            public string regionId;
#pragma warning restore RAD201
        }

        public class TokenExchangeResponse
        {
#pragma warning disable RAD201
            public string accessToken;
#pragma warning restore RAD201
        }

        // API is described here:
        // https://services.docs.unity.com/multiplay-config/v1/#tag/Allocations

        // todo: https://docs.unity.com/ugs/en-us/manual/game-server-hosting/manual/sdk/game-server-sdk-for-unity

        const string KeyId = "89302e22-e73b-4890-80fd-04e29f27a721";
        const string KeySecret = "cQb6Cj1nV6QyIsflmwVxt-ZTLtEpa8_P";
        const string ProjectId = "f99e7a47-7455-4b68-a7fd-0b4c6e99c755";
        const string EnvironmentId = "3a7f6955-55a9-4574-94b0-8c4ef77f8666";
        const string FleetId = "89aa5df1-6886-45e4-b808-a1b67563367a";
        const string RegionId = "436932c9-100c-4156-b12d-7b8e507a9a9e";
        const int BuildConfigurationID = 1270962;

        /// <summary>
        /// Indicates if the system is currently processing a call. Consecutive calls are not allowed.
        /// </summary>
        internal static bool RequestInProgress
        {
            get;
            private set;
        }

        /// <summary>
        /// The result of the last call. Either the requested resource (as JSON) or the error message.
        /// </summary>
        static string _result;

        /// <summary>
        /// Tell if the last call has been successful.
        /// </summary>
        static bool _error;

        /// <summary>
        /// Allocate allocation to the server
        /// </summary>
        static string _allocationId;

        /// <summary>
        /// Returns a list of as a callback function.
        /// </summary>
        internal static async Task<List<ServerDto>> GetServers()
        {
            Assert.IsFalse(RequestInProgress, "Consecutive calls are not allowed");

            RequestInProgress = true;
            string url = $"https://services.api.unity.com/multiplay/servers/v1/projects/{ProjectId}/environments/{EnvironmentId}/servers";
            StaticCoroutine.StartStaticCoroutine(GetServersCoroutine(url));

            while (RequestInProgress)
                await Task.Yield();

            var retVal = new List<ServerDto>();

            if (_error)
            {
                Debug.LogError("Error: " + _error);
                return retVal;
            }

            var servers = JsonUtility.FromJson<ServerListDto>("{\"ServerList\":" + _result + "}");

            if (servers.ServerList == null)
                return retVal;

            retVal.AddRange(servers.ServerList);
            return retVal;
        }

        internal static async Task<bool> CreateTestAllocation()
        {
            List<AllocationDto> allocations = await GetTestAllocations();
            AllocationDto? freeAllocation = allocations.FirstOrDefault(allocation => string.IsNullOrEmpty(allocation.requested));
            if (freeAllocation == null)
            {
                Debug.LogWarning("All allocations are full, create more allocations in unity.cloud");
                return false;
            }
            _allocationId = freeAllocation.allocationId;
            return true;
        }

        internal static async Task<List<AllocationDto>> GetTestAllocations()
        {
            Assert.IsFalse(RequestInProgress, "Consecutive calls are not allowed");

            RequestInProgress = true;
            string url = $"https://services.api.unity.com/multiplay/allocations/v1/projects/{ProjectId}/environments/{EnvironmentId}/test-allocations";
            StaticCoroutine.StartStaticCoroutine(GetTestAllocationsCoroutine(url));

            while (RequestInProgress)
                await Task.Yield();

            var retVal = new List<AllocationDto>();

            if (_error)
            {
                Debug.LogError("Error: " + _error);
                return retVal;
            }

            var allocations = JsonUtility.FromJson<AllocationListDto>(_result);

            if (allocations.allocations == null)
                return retVal;

            retVal.AddRange(allocations.allocations);
            return retVal;
        }

        internal static void SetConnectionData()
        {
            // todo: add ip and port
            //NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("ipv4Address", "port");
        }

        internal static async Task CreateServer()
        {
            string url = $"https://services.api.unity.com/auth/v1/token-exchange?projectId={ProjectId}&environmentId={EnvironmentId}";

            string jsonRequestBody = JsonUtility.ToJson(new TokenExchangeRequest
            {
                scopes = new[] { "multiplay.allocations.create", "multiplay.allocations.list" },
            });

            // todo: here must start the coroutine because yes
            StaticCoroutine.StartStaticCoroutine(CreateServersCoroutine(url, jsonRequestBody));

            while (RequestInProgress)
                await Task.Yield();
        }

        /// <summary>
        /// This coroutine will eventually fill the <see cref="_result"/> variable and change <see cref="RequestInProgress"/> to false.
        /// </summary>
        static IEnumerator GetServersCoroutine(string url)
        {
            using UnityWebRequest request = UnityWebRequest.Get(url);

            byte[] keyByteArray = Encoding.UTF8.GetBytes(KeyId + ":" + KeySecret);
            string keyBase64 = Convert.ToBase64String(keyByteArray);

            request.SetRequestHeader("Authorization", "Basic " + keyBase64);

            yield return request.SendWebRequest();

            if (request.result is UnityWebRequest.Result.ConnectionError
                                  or UnityWebRequest.Result.DataProcessingError
                                  or UnityWebRequest.Result.ProtocolError)
            {
                _error = true;
                _result = request.error;
            }
            else
            {
                _error = false;
                _result = request.downloadHandler.text;
            }

            RequestInProgress = false;
        }

        static IEnumerator CreateServersCoroutine(string url, string jsonRequestBody)
        {
            using var request = new UnityWebRequest(url, "POST");

            byte[] keyByteArray = Encoding.UTF8.GetBytes(KeyId + ":" + KeySecret);
            string keyBase64 = Convert.ToBase64String(keyByteArray);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequestBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Basic " + keyBase64);

            yield return request.SendWebRequest();

            if (request.result is UnityWebRequest.Result.ConnectionError
                                  or UnityWebRequest.Result.DataProcessingError
                                  or UnityWebRequest.Result.ProtocolError)
            {
                _error = true;
                _result = request.error;
                Debug.Log("CreateServersCoroutine Error: " + request.error);
                RequestInProgress = false;
            }
            else
            {
                _error = false;
                _result = request.downloadHandler.text;
                Debug.Log("CreateServersCoroutine Success: " + request.downloadHandler.text);

                string urlInternal = $"https://multiplay.services.api.unity.com/v1/allocations/projects/{ProjectId}/environments/{EnvironmentId}/fleets/{FleetId}/allocations";
                StaticCoroutine.StartStaticCoroutine(CreateServersCoroutine_Internal(urlInternal, request.downloadHandler.text));
            }
        }

        /// <summary>
        /// This coroutine will eventually fill the <see cref="_result"/> variable and change <see cref="RequestInProgress"/> to false.
        /// </summary>
        static IEnumerator CreateServersCoroutine_Internal(string url, string jsonRequestBody)
        {
            var tokenExchangeResponse = JsonUtility.FromJson<TokenExchangeResponse>(jsonRequestBody);
            using var request = new UnityWebRequest(url, "POST");

            string jsonData = JsonUtility.ToJson(new AllocationRequest
            {
                allocationId = _allocationId,
                buildConfigurationId = BuildConfigurationID,
                regionId = RegionId,
            });

            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + tokenExchangeResponse.accessToken);

            yield return request.SendWebRequest();

            if (request.result is UnityWebRequest.Result.ConnectionError
                                  or UnityWebRequest.Result.DataProcessingError
                                  or UnityWebRequest.Result.ProtocolError)
            {
                _error = true;
                _result = request.error;
                Debug.Log("CreateServersCoroutine_Internal Error: " + request.error);
            }
            else
            {
                _error = false;
                _result = request.downloadHandler.text;
                Debug.Log("CreateServersCoroutine_Internal Success: " + request.downloadHandler.text);
            }

            RequestInProgress = false;
        }

        static IEnumerator GetTestAllocationsCoroutine(string url)
        {
            using UnityWebRequest request = UnityWebRequest.Get(url);

            byte[] keyByteArray = Encoding.UTF8.GetBytes(KeyId + ":" + KeySecret);
            string keyBase64 = Convert.ToBase64String(keyByteArray);

            request.SetRequestHeader("Authorization", "Basic " + keyBase64);

            yield return request.SendWebRequest();

            if (request.result is UnityWebRequest.Result.ConnectionError
                                  or UnityWebRequest.Result.DataProcessingError
                                  or UnityWebRequest.Result.ProtocolError)
            {
                _error = true;
                _result = request.error;
                Debug.Log("GetTestAllocationsCoroutine Error: " + request.error);
            }
            else
            {
                _error = false;
                _result = request.downloadHandler.text;
                Debug.Log("GetTestAllocationsCoroutine Success: " + request.downloadHandler.text);
            }

            RequestInProgress = false;
        }
    }
}