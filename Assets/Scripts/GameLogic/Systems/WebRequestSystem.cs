#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Dtos;
using Shared;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace GameLogic.Systems
{
    static class WebRequestSystem
    {
        // todo: https://docs.unity.com/ugs/en-us/manual/game-server-hosting/manual/sdk/game-server-sdk-for-unity

        const string KeyId = "89302e22-e73b-4890-80fd-04e29f27a721";
        const string KeySecret = "cQb6Cj1nV6QyIsflmwVxt-ZTLtEpa8_P";
        const string ProjectId = "f99e7a47-7455-4b68-a7fd-0b4c6e99c755";
        const string EnvironmentId = "3a7f6955-55a9-4574-94b0-8c4ef77f8666";
        const string FleetId = "89aa5df1-6886-45e4-b808-a1b67563367a";

        /// <summary>
        /// The result of the last call. Either the requested resource (as JSON) or the error message.
        /// </summary>
        static string _result;

        /// <summary>
        /// Indicates if the system is currently processing a call. Consecutive calls are not allowed.
        /// </summary>
        static bool _requestInProgress;

        /// <summary>
        /// Tell if the last call has been successful.
        /// </summary>
        static bool _error;

        /// <summary>
        /// Returns a list of as a callback function.
        /// </summary>
        internal static async Task<List<ServerDto>> GetServers()
        {
            Assert.IsFalse(_requestInProgress, "Consecutive calls are not allowed");

            _requestInProgress = true;
            string url = $"https://services.api.unity.com/multiplay/servers/v1/projects/{ProjectId}/environments/{EnvironmentId}/servers";
            StaticCoroutine.StartStaticCoroutine(GetServersCoroutine(url));

            while (_requestInProgress)
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

        internal static void SetConnectionData()
        {
            // todo: add ip and port
            //NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("ipv4Address", "port");
        }

        /// <summary>
        /// This coroutine eventually fill the variable.
        /// </summary>
        static IEnumerator GetServersCoroutine(string url)
        {
            using UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);

            byte[] keyByteArray = Encoding.UTF8.GetBytes(KeyId + ":" + KeySecret);
            string keyBase64 = Convert.ToBase64String(keyByteArray);

            unityWebRequest.SetRequestHeader("Authorization", "Basic " + keyBase64);

            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result is UnityWebRequest.Result.ConnectionError
                                          or UnityWebRequest.Result.DataProcessingError
                                          or UnityWebRequest.Result.ProtocolError)
            {
                _error = true;
                _result = unityWebRequest.error;
            }
            else
            {
                _error = false;
                _result = unityWebRequest.downloadHandler.text;
            }

            _requestInProgress = false;
        }
    }
}