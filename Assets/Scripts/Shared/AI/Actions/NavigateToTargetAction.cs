using System;
using JetBrains.Annotations;
using Shared.AI.Interfaces;
using UnityEngine;

namespace Shared.AI.Actions
{
    /// <summary>
    /// A state machine action that navigates a character controller to specified target
    /// </summary>
    public class NavigateToTargetAction : StateMachineActionBase, INavigateAction
    {
        /// <summary>
        /// If the character cannot reach the desired target exactly and the displacement is more than this value, the action
        /// will fail
        /// </summary>
        public float MaxNavigationDisplacement = 1f;

        /// <summary>
        /// Optional. If set and the calculated navigation distance is bigger than this value, the action will fail
        /// </summary>
        public float? MaxNavigationDistance;

        /// <summary>
        /// Optional. If set and the navigation takes more than this time, the action will fail
        /// </summary>
        public TimeSpan? MaxNavigationTime;

        /// <summary>
        /// If set, the target is refreshed with this interval. Useless in case of static targets, required in case of dynamic
        /// targets
        /// </summary>
        public TimeSpan? TargetPathRefreshInterval;

        /// <summary>
        /// Optional. A curve that defines navigation velocity based on distance to target
        /// </summary>
        [CanBeNull]
        public AnimationCurve DistanceToVelocityMap;

        protected DateTime ActionStartTime;

        DateTime _lastTargetRefreshTime;
        readonly INavigationTarget _target;
        readonly NavMeshNavigationController _controller;

        Vector3 _targetPosition;
        float? _targetYaw;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controller">Controller used for navigation</param>
        /// <param name="target">Navigation target</param>
        public NavigateToTargetAction([NotNull] NavMeshNavigationController controller, [NotNull] INavigationTarget target)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public Vector3? TargetPosition
        {
            get
            {
                if (CurrentState != StateMachineActionState.Awaiting)
                    return _targetPosition;

                _target.GetTarget(_controller.CurrentPosition, _controller.GetCurrentYaw(), out Vector3 targetPosition, out _);
                return targetPosition;
            }
        }

        public override void Update()
        {
            switch (CurrentState)
            {
                case StateMachineActionState.InProgress:
                {
                    HandleProgress();
                    break;
                }
                case StateMachineActionState.Finishing:
                    FinalizeAction(true);
                    break;
            }
        }

        public override void Start()
        {
            base.Start();

            if (!RefreshTargetDestination())
                return;

            SetVelocity();
            _controller.IsActive = true;
            ActionStartTime = DateTime.Now;
            CurrentState = StateMachineActionState.InProgress;
        }

        protected virtual void FinalizeAction(bool succeeded, string failureReason = null)
        {
            if (!succeeded)
            {
                if (string.IsNullOrEmpty(failureReason))
                    throw new ArgumentNullException(nameof(failureReason));

                FailureReason = failureReason;
            }

            CurrentState = succeeded
                ? StateMachineActionState.Succeeded
                : StateMachineActionState.Failed;

            _controller.IsActive = false;
        }

        void HandleProgress()
        {
            if (MaxNavigationTime.HasValue && DateTime.Now - ActionStartTime > MaxNavigationTime.Value)
            {
                FinalizeAction(false, "Navigation timeout");
                return;
            }

            if (TargetPathRefreshInterval.HasValue
                && (!_controller.IsNavigating || DateTime.Now - _lastTargetRefreshTime > TargetPathRefreshInterval.Value))
                if (!RefreshTargetDestination())
                    return;

            if (_controller.IsNavigating)
            {
                if (MaxNavigationDistance.HasValue && MaxNavigationDistance.Value < _controller.RemainingDistance)
                    FinalizeAction(false, "Exceeded max navigation distance");

                SetVelocity();
                return;
            }

            Vector3 currentPosition = _controller.CurrentPosition;
            currentPosition.y = _targetPosition.y;
            if (Vector3.Distance(_targetPosition, currentPosition) > MaxNavigationDisplacement)
                FinalizeAction(false, "Cannot reach target");
            else
                FinalizeAction(true);
        }

        void SetVelocity()
        {
            if (DistanceToVelocityMap == null || float.IsInfinity(_controller.RemainingDistance) || float.IsNaN(_controller.RemainingDistance))
                return;

            float targetVelocity = DistanceToVelocityMap.Evaluate(_controller.RemainingDistance);
            _controller.TargetLinearVelocity = targetVelocity;
        }

        bool RefreshTargetDestination()
        {
            Vector3 currentPosition = _controller.CurrentPosition;
            float currentYaw = _controller.GetCurrentYaw();

            _target.GetTarget(currentPosition, currentYaw, out _targetPosition, out _targetYaw);

            if (!_controller.SetTarget(_targetPosition, _targetYaw))
            {
                FinalizeAction(false, "Cannot find path to target");
                return false;
            }

            _lastTargetRefreshTime = DateTime.Now;
            return true;
        }
    }
}