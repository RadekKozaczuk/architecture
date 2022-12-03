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