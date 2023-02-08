using Sirenix.OdinInspector;
using UI.Views;
using UnityEngine;

namespace UI.Config
{
    [CreateAssetMenu(fileName = "UIDebugConfig", menuName = "Config/UI/DebugCommands/UIDebugConfig")]
    class UIDebugConfig : ScriptableObject
    {
        [SerializeField]
        [InfoBox("Duration between 3 clicks in seconds", InfoMessageType.None)]
        [Range(0.01f, 0.5f)]
        [SuffixLabel("s")]
        internal float TripleClickDuration = 0.5f;

        [SerializeField]
        internal DebugMobileButtonView MobileButtonPrefab;

        [SerializeField]
        internal DebugMobileConsoleView MobileConsolePrefab;

        [SerializeField]
        internal DebugConsoleView ConsolePrefab;

        [SerializeField]
        internal GameObject CommandPrefab;
    }
}