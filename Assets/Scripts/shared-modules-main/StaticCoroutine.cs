using System;
using System.Collections;
using UnityEngine;

namespace Shared
{
    /// <summary>
    /// In Unity only enabled <see cref="MonoBehaviour" />s can start a coroutine.
    /// This class helps us avoid this limitation.
    /// </summary>
    [DisallowMultipleComponent]
    public class StaticCoroutine : MonoBehaviour
    {
        static StaticCoroutine _instance;

        void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static Coroutine StartStaticCoroutine(IEnumerator coroutine, Action onComplete) =>
            _instance.StartCoroutine(_instance.PerformWithCallback(coroutine, onComplete));

        public static Coroutine StartStaticCoroutine(IEnumerator coroutine) => _instance.StartCoroutine(coroutine);

        public static void StopStaticCoroutine(Coroutine coroutine) => _instance.StopCoroutine(coroutine);

        IEnumerator PerformWithCallback(IEnumerator coroutine, Action onComplete = null)
        {
            yield return StartCoroutine(coroutine);

            onComplete?.Invoke();
        }
    }
}