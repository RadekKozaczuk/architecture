#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Shared;
using UnityEngine;
using UnityEngine.Networking;

namespace UI.Systems
{
    public static class WebRequestSystem
    {
        class WebRequestsMonoBehaviour : MonoBehaviour { }

        public static void Get(string url, Action<string> onError, Action<string> onSuccess)
        {
            StaticCoroutine.StartStaticCoroutine(GetCoroutine(url, null, onError, onSuccess));
        }

        public static void Get(string url, Action<UnityWebRequest> setHeaderAction, Action<string> onError, Action<string> onSuccess)
        {
            StaticCoroutine.StartStaticCoroutine(GetCoroutine(url, setHeaderAction, onError, onSuccess));
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

        public static void Post(string url, Dictionary<string, string> formFields, Action<string> onError, Action<string> onSuccess)
        {
            StaticCoroutine.StartStaticCoroutine(GetCoroutinePost(url, formFields, onError, onSuccess));
        }

        public static void Post(string url, string postData, Action<string> onError, Action<string> onSuccess)
        {
            StaticCoroutine.StartStaticCoroutine(GetCoroutinePost(url, postData, onError, onSuccess));
        }

        public static void PostJson(string url, string jsonData, Action<string> onError, Action<string> onSuccess)
        {
            StaticCoroutine.StartStaticCoroutine(GetCoroutinePostJson(url, null, jsonData, onError, onSuccess));
        }

        public static void PostJson(string url, Action<UnityWebRequest> setHeaderAction, string jsonData, Action<string> onError, Action<string> onSuccess) 
        {
            StaticCoroutine.StartStaticCoroutine(GetCoroutinePostJson(url, setHeaderAction, jsonData, onError, onSuccess));
        }

        static IEnumerator GetCoroutinePost(string url, Dictionary<string, string> formFields, Action<string> onError, Action<string> onSuccess)
        {
            using UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, formFields);
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

        static IEnumerator GetCoroutinePost(string url, string postData, Action<string> onError, Action<string> onSuccess)
        {
            using UnityWebRequest unityWebRequest = UnityWebRequest.PostWwwForm(url, postData);
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

        public static void Put(string url, string bodyData, Action<string> onError, Action<string> onSuccess)
        {
            StaticCoroutine.StartStaticCoroutine(GetCoroutinePut(url, bodyData, onError, onSuccess));
        }

        static IEnumerator GetCoroutinePut(string url, string bodyData, Action<string> onError, Action<string> onSuccess)
        {
            using UnityWebRequest unityWebRequest = UnityWebRequest.Put(url, bodyData);
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

        public static void GetTexture(string url, Action<string> onError, Action<Texture2D> onSuccess)
        {
            StaticCoroutine.StartStaticCoroutine(GetTextureCoroutine(url, onError, onSuccess));
        }

        static IEnumerator GetTextureCoroutine(string url, Action<string> onError, Action<Texture2D> onSuccess)
        {
            using UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url);
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
                var downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
                onSuccess(downloadHandlerTexture!.texture);
            }
        }
    }
}