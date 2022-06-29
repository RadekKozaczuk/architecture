using UnityEngine;

namespace Shared.AI.Actions
{
    /// <summary>
    /// Represents a navigation target
    /// </summary>
    public interface INavigationTarget
    {
        /// <summary>
        /// Calculates the navigation target based on current state and implementation specific logic
        /// </summary>
        /// <param name="currentPosition">Current reference position</param>
        /// <param name="currentYaw">Current reference yaw in degrees</param>
        /// <param name="targetPosition">Calculated target position</param>
        /// <param name="targetYaw">Calculated target yaw in degrees</param>
        void GetTarget(Vector3 currentPosition, float currentYaw, out Vector3 targetPosition, out float? targetYaw);
    }
}