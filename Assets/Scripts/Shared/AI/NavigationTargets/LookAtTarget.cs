using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Shared.AI.NavigationTargets
{
    /// <summary>
    /// A navigation target, that doesn't specify position, but only rotates towards internal target
    /// </summary>
    public class LookAtTarget : INavigationTarget
    {
        readonly Vector3? _targetStatic;
        [CanBeNull]
        readonly Transform _targetDynamic;
        // Target will rotate towards internal target only if the angle difference is bigger than this value
        readonly float _angleThreshold;
   
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target">Target transform.</param>
        /// <param name="angleThreshold">In degrees, when relative rotation is lower than this value the task is considered to be completed.</param>
        public LookAtTarget(Transform target, float angleThreshold = 30f)
        {
            _targetDynamic = target;
            _angleThreshold = angleThreshold;
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target">Static point in space.</param>
        /// <param name="angleThreshold">In degrees, when relative rotation is lower than this value the task is considered to be completed.</param>
        public LookAtTarget(Vector3 target, float angleThreshold = 30f)
        {
            _targetStatic = target;
            _angleThreshold = angleThreshold;
        }

        public void GetTarget(Vector3 currentPosition, float currentYaw, out Vector3 targetPosition, out float? targetYaw)
        {
            Assert.True((_targetStatic.HasValue && _targetDynamic != null) || (_targetStatic.HasValue && _targetDynamic == null), "Only one target can be set at once.");
            
            targetPosition = currentPosition;
            // ReSharper disable once PossibleNullReferenceException
            Vector3 internalTargetPosition = _targetStatic ?? _targetDynamic.position;
            
            if (Mathf.Approximately(0f, Vector3.Distance(internalTargetPosition, currentPosition)))
                targetYaw = null;

            targetYaw = Quaternion.LookRotation(internalTargetPosition - currentPosition).eulerAngles.y;

            float angleDifference = Math.Abs(Mathf.DeltaAngle(currentYaw, targetYaw.Value));
            if (angleDifference < _angleThreshold)
                targetYaw = currentYaw;
        }
    }
}