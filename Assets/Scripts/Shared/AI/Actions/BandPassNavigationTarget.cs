using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Shared.AI.Actions
{
    /// <summary>
    /// A navigation target that adds filtering based on current state and other navigation target
    /// </summary>
    public class BandPassNavigationTarget : INavigationTarget
    {
        /// <summary>
        /// High pass threshold. If the distance between current position and target position is bigger than this value, target
        /// will always be followed
        /// </summary>
        public float HighPassLinearThreshold = 5f;

        /// <summary>
        /// Low pass filtering options
        /// </summary>
        public LowPassFilter LowPass { get; } = new();

        readonly INavigationTarget _target;

        [CanBeNull]
        readonly INavigationTarget _filteredTarget;

        Vector3? _targetPosition;
        float? _targetYaw;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">Reference target to which filtering is applied</param>
        /// <param name="filteredTarget">
        /// Optional. If <paramref name="target" /> movement is filtered out, this target is used
        /// instead
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public BandPassNavigationTarget([NotNull] INavigationTarget target, [CanBeNull] INavigationTarget filteredTarget = null)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _filteredTarget = filteredTarget;
        }

        public void GetTarget(Vector3 currentPosition, float currentYaw, out Vector3 targetPosition, out float? targetYaw)
        {
            _target.GetTarget(currentPosition, currentYaw, out Vector3 internalTargetPosition, out float? internalTargetYaw);

            if (!_targetPosition.HasValue || Vector3.Distance(internalTargetPosition, currentPosition) > HighPassLinearThreshold)
            {
                _targetPosition = internalTargetPosition;
                _targetYaw = internalTargetYaw;
            }
            else
            {
                if (!LowPass.IsFiltered(internalTargetPosition, internalTargetYaw))
                {
                    _targetPosition = internalTargetPosition;
                    _targetYaw = internalTargetYaw;
                }
                else if (_filteredTarget != null)
                {
                    _filteredTarget.GetTarget(_targetPosition.Value, currentYaw, out targetPosition, out targetYaw);
                    return;
                }
            }

            targetPosition = _targetPosition.Value;
            targetYaw = _targetYaw;
        }
    }
}