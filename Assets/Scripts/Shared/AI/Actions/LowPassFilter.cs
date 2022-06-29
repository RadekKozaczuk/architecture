using System;
using UnityEngine;

namespace Shared.AI.Actions
{
    /// <summary>
    /// Represents a Low Pass filter for navigation targets
    /// </summary>
    public class LowPassFilter
    {
        /// <summary>
        /// Filter will stop filtering if there's no movement within this period of time
        /// </summary>
        public TimeSpan ResponseDelay = TimeSpan.FromSeconds(3);

        /// <summary>
        /// Below this value filter will consider, that there's no movement
        /// </summary>
        public float AngularThreshold = 30f;

        /// <summary>
        /// Below this value filter will consider, that there's no movement
        /// </summary>
        public float LinearThreshold = 2f;

        Vector3? _lowPassPosition;
        float? _lowPassYaw;
        float _lowPassTime;

        public bool IsFiltered(Vector3 position, float? yaw)
        {
            if (!yaw.HasValue)
                _lowPassYaw = null;

            if (_lowPassPosition.HasValue
                && !(Vector3.Distance(_lowPassPosition.Value, position) > LinearThreshold)
                && (!yaw.HasValue || _lowPassYaw.HasValue && !(Mathf.DeltaAngle(_lowPassYaw.Value, yaw.Value) > AngularThreshold)))
                return !(Time.time > _lowPassTime + ResponseDelay.TotalSeconds);

            _lowPassPosition = position;
            _lowPassYaw = yaw;
            _lowPassTime = Time.time;
            return true;
        }
    }
}