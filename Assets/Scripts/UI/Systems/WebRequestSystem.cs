#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using System.Collections;
using System.Text;
using Shared;
using UnityEngine.Networking;

namespace UI.Systems
{
    public static class WebRequestSystem
    {
        public static void Get(string url, Action<UnityWebRequest> setHeaderAction, Action<string> onError, Action<string> onSuccess)
        {
            StaticCoroutine.StartStaticCoroutine(GetCoroutine(url, setHeaderAction, onError, onSuccess));
        }

        public static void PostJson(string url, Action<UnityWebRequest> setHeaderAction, string jsonData, Action<string> onError, Action<string> onSuccess) 
        {
            StaticCoroutine.StartStaticCoroutine(GetCoroutinePostJson(url, setHeaderAction, jsonData, onError, onSuccess));
        }

        static IEnumerator GetCoroutine(string url, Action<UnityWebRequest>? setHeaderAction, Action<string> onError, Action<string> onSuccess)
        {
            using UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
            setHeaderAction?.Invoke(unityWebRequest);

            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result is UnityWebRequest.Result.ConnectionError
                                          or UnityWebRequest.Result.DataProcessingError
                                          or UnityWebRequest.Result.ProtocolError)
            {
                // Error
                onError(unityWebRequest.error);
            }
            else
            {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }

        static IEnumerator GetCoroutinePostJson(string url, Action<UnityWebRequest>? setHeaderAction, string jsonData, Action<string> onError, Action<string> onSuccess)
        {
            using var unityWebRequest = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            setHeaderAction?.Invoke(unityWebRequest);

            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result is UnityWebRequest.Result.ConnectionError
                                          or UnityWebRequest.Result.DataProcessingError
                                          or UnityWebRequest.Result.ProtocolError)
            {
                // Error
                onError(unityWebRequest.error);
            }
            else
            {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }
    }
}