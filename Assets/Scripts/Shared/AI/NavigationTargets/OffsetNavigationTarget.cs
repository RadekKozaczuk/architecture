using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Shared.AI.NavigationTargets
{
    /// <summary>
    /// Navigation target that is defined as an offset from other navigation target
    /// </summary>
    public class OffsetNavigationTarget : INavigationTarget
    {
        readonly INavigationTarget _target;
        readonly float _approachDistance;
        readonly float? _approachAngle;
        readonly float _targetRelativeYaw;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">The target from which the offset will be calculated.</param>
        /// <param name="approachDistance">
        /// Offset to <paramref name="target" />. If <paramref name="target" /> specifies its
        /// orientation, the target position is calculated as an offset in this direction.
        /// </param>
        /// <param name="approachAngle">
        /// Optional, degrees. If specified, this angle is added to the orientation of
        /// <paramref name="target" /> to specify offset direction. If the <paramref name="target" /> doesn't specify
        /// orientation, the offset direction is based on navigation path.
        /// </param>
        /// <param name="targetRelativeYaw">
        /// Optional, degrees. If the <paramref name="target" /> specifies orientation, the calculated orientation will be the
        /// sum of of <paramref name="target" /> orientation and this angle. If the <paramref name="target" /> doesn't specify
        /// orientation, the orientation will be calculated to face the <paramref name="target" /> plus this angle.
        /// </param>
        public OffsetNavigationTarget([NotNull] INavigationTarget target, float approachDistance,
            float? approachAngle = null, float targetRelativeYaw = 0f)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _approachDistance = approachDistance;
            _approachAngle = approachAngle;
            _targetRelativeYaw = targetRelativeYaw;
        }

        public void GetTarget(Vector3 currentPosition, float currentYaw, out Vector3 targetPosition, out float? targetYaw)
        {
            _target.GetTarget(currentPosition, currentYaw, out Vector3 internalTargetPosition, out float? internalTargetYaw);

            if (!internalTargetYaw.HasValue || !_approachAngle.HasValue)
                targetPosition = Vector3.MoveTowards(internalTargetPosition, currentPosition, _approachDistance);
            else
                targetPosition = internalTargetPosition
                                 + Quaternion.AngleAxis(internalTargetYaw.Value + _approachAngle.Value, Vector3.up)
                                 * Vector3.forward
                                 * _approachDistance;

            if (internalTargetYaw.HasValue)
                targetYaw = internalTargetYaw + _targetRelativeYaw;
            else
            {
                if (Mathf.Approximately(0f, Vector3.Distance(internalTargetPosition, targetPosition)))
                    targetYaw = null;
                else
                {
                    Vector3 direction = internalTargetPosition - targetPosition;
                    targetYaw = Quaternion.LookRotation(direction, Vector3.up).eulerAngles.y + _targetRelativeYaw;
                }
            }
        }
    }
}