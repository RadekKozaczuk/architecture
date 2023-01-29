#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && (UNITY_ANDROID || UNITY_IPHONE)
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Config
{
    [CreateAssetMenu(fileName = "UIDebugConfig", menuName = "Config/UI/DebugCommands/UIDebugConfig")]
    class UIDebugConfig : ScriptableObject
    {
        [SerializeField]
        [InfoBox("Duration between 3 clicks in seconds")]
        [Range(0.01f, 0.5f)]
        [SuffixLabel("s")]
        internal float TripleClickDuration = 0.5f;
    }
}
#endif