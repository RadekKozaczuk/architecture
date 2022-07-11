using System;
using Shared.Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Shared.AI
{
    public class NavMeshNavigationController : ICustomUpdate
    {
        public float TargetYawSetupFactor { get => _targetYawSetupFactor; set => _targetYawSetupFactor = Mathf.Clamp01(value); }

        public Vector3? TargetPosition { get; private set; }
        public float? TargetYaw { get; private set; }
        public Vector3 CurrentPosition => _agent.transform.position;

        public bool IsActive { get => !_agent.isStopped; set => _agent.isStopped = !value; }

        public bool IsNavigating
        {
            get
            {
                if (!TargetPosition.HasValue && !TargetYaw.HasValue)
                    return false;

                if (_agent.pathPending)
                    return true;

                bool targetPositionReached = !TargetPosition.HasValue || Mathf.Approximately(_agent.remainingDistance, 0f);
                bool targetRotationReached = !TargetYaw.HasValue || Mathf.Approximately(_agent.transform.rotation.eulerAngles.y, TargetYaw.Value);

                return !(targetPositionReached && targetRotationReached);
            }
        }

        public float RemainingDistance => _agent.remainingDistance;

        public Quaternion CurrentRotation => _agent.transform.rotation;

        public float TargetLinearVelocity { get => _agent.speed; set => _agent.speed = value; }

        public float TargetAngularVelocity { get => _agent.angularSpeed; set => _agent.angularSpeed = value; }
        
        readonly NavMeshAgent _agent;
        float _targetYawSetupFactor = 0.5f;

        public NavMeshNavigationController(NavMeshAgent agent) => _agent = agent;

        public bool SetTarget(Vector3? position, float? targetYaw)
        {
            if (position.HasValue)
                if (!_agent.SetDestination(position.Value))
                    return false;

            TargetPosition = position;
            TargetYaw = (float?)Mathd.NormalizeAngleDegrees(targetYaw);
            return true;
        }

        public void CustomUpdate() => HandleTargetRotation();

        void HandleTargetRotation()
        {
            if (!TargetYaw.HasValue || Mathf.Approximately(_agent.angularSpeed, 0f))
            {
                _agent.updateRotation = true;
                return;
            }

            Quaternion currentRotation = _agent.transform.rotation;
            Vector3 currentRotationEuler = currentRotation.eulerAngles;
            float angularDifference = Math.Abs(Mathf.DeltaAngle(currentRotationEuler.y, TargetYaw.Value));
            if (Mathf.Approximately(angularDifference, 0f))
            {
                _agent.updateRotation = true;
                return;
            }

            float timeToRotate = angularDifference / _agent.angularSpeed;
            float remainingDistance = _agent.remainingDistance;
            float remainingTime = Mathf.Approximately(_agent.speed, 0f) ? 0f : remainingDistance / _agent.speed;

            if (timeToRotate > 0 && remainingTime / timeToRotate <= Mathf.Clamp01(TargetYawSetupFactor))
            {
                _agent.updateRotation = false;
                Quaternion targetRotation = Quaternion.Euler(currentRotationEuler.x, TargetYaw.Value, currentRotationEuler.z);
                _agent.transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, Time.deltaTime * _agent.angularSpeed);
            }
            else
                _agent.updateRotation = true;
        }
    }
}