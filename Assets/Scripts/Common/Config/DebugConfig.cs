#if UNITY_EDITOR || DEVELOPMENT_BUILD
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable
using Sirenix.OdinInspector;
using UnityEngine;

namespace Common.Config
{
    [CreateAssetMenu(fileName = "DebugConfig", menuName = "Config/Common/DebugConfig")]
    public class DebugConfig : ScriptableObject
    {
        /// <summary>
        /// Show the order of execution of all reactive (implementing React attribute) systems and controllers.
        /// </summary>
        public bool LogSystemExecutionOrder;

        /// <summary>
        /// Logs all added <see cref="Shared.AbstractSignal" />.
        /// </summary>
        public bool LogAddedSignals;

        /// <summary>
        /// Logs all state change requests.
        /// </summary>
        [InfoBox("Logs GameState change request and what scenes are requested to be loaded and unloaded.", InfoMessageType.None)]
        public bool LogRequestedStateChange;

        /// <summary>
        /// Logs player position relative to search point
        /// </summary>
        public bool LogPlayerPosition;

        /// <summary>
        /// Shows Input log.
        /// </summary>
        [InfoBox("Shows input registered.", InfoMessageType.None)]
        public bool ShowInputLog;
    }
}
#endif