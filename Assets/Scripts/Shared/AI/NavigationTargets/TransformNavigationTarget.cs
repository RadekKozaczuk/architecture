using JetBrains.Annotations;
using UnityEngine;

namespace Shared.AI.NavigationTargets
{
    /// <summary>
    /// Navigation target defined by a transform.
    /// </summary>
    public class TransformNavigationTarget : INavigationTarget
    {
        readonly Transform _transform;
        readonly bool _useTargetOrientation;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="transform">Transform that provides navigation target</param>
        /// <param name="useTargetOrientation">True means that target's transform orientation will be taken into account when calculating target, false means that the orientation does not matter.</param>
        public TransformNavigationTarget([NotNull] Transform transform, bool useTargetOrientation)
        {
            Assert.False(transform == null, "Transform cannot be null.");

            _transform = transform;
            _useTargetOrientation = useTargetOrientation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="currentYaw"></param>
        /// <param name="targetPosition"></param>
        /// <param name="targetYaw">Target's transform orientation when useTargetOrientation set to true, null when set to false.</param>
        public void GetTarget(Vector3 currentPosition, float currentYaw, out Vector3 targetPosition, out float? targetYaw)
        {
            targetPosition = _transform.position;
            targetYaw = _useTargetOrientation
                ? _transform.rotation.eulerAngles.y
                : null;
        }
    }
}