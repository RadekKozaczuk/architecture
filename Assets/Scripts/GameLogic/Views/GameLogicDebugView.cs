#if UNITY_EDITOR
using UnityEngine;

namespace GameLogic.Views
{
    [DisallowMultipleComponent]
    public class GameLogicDebugView : MonoBehaviour
    {
        void Awake() => DontDestroyOnLoad(gameObject);
    }
}
#endif