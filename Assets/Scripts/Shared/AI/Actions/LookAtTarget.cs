using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Shared.AI.Actions
{
    /// <summary>
    /// A navigation target, that doesn't specify position, but only rotates towards internal target
    /// </summary>
    public class LookAtTarget : INavigationTarget
    {
        /// <summary>
        /// Target will rotate towards internal target only if the angle difference is bigger than this value
        /// </summary>
        public double AngleThreshold = 30f;

        readonly INavigationTarget _target;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">The target towards which this target will rotate</param>
        public LookAtTarget([NotNull] INavigationTarget target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public void GetTarget(Vector3 currentPosition, float currentYaw, out Vector3 targetPosition, out float? targetYaw)
        {
            targetPosition = currentPosition;
            _target.GetTarget(currentPosition, currentYaw, out Vector3 internalTargetPosition, out _);

            if (Mathf.Approximately(0f, Vector3.Distance(internalTargetPosition, currentPosition)))
                targetYaw = null;

            targetYaw = Quaternion.LookRotation(internalTargetPosition - currentPosition).eulerAngles.y;

            float angleDifference = Math.Abs(Mathf.DeltaAngle(currentYaw, targetYaw.Value));
            if (angleDifference < AngleThreshold)
                targetYaw = currentYaw;
        }
    }
}