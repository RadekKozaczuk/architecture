using UnityEngine;

namespace Shared.AI.NavigationTargets
{
    /// <summary>
    /// A statically defined navigation target
    /// </summary>
    public class StaticNavigationTarget : INavigationTarget
    {
        readonly Vector3? _targetPosition;
        readonly float? _targetYaw;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="targetPosition">Desired target position</param>
        /// <param name="targetYaw">Desired target yaw in degrees, in degrees, clockwise</param>
        public StaticNavigationTarget(Vector3? targetPosition, float? targetYaw = null)
        {
            _targetPosition = targetPosition;
            _targetYaw = targetYaw;
        }

        public void GetTarget(Vector3 currentPosition, float currentYaw, out Vector3 targetPosition, out float? targetYaw)
        {
            targetPosition = _targetPosition ?? currentPosition;
            targetYaw = _targetYaw;
        }
    }
}