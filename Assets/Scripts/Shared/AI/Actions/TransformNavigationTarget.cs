using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Shared.AI.Actions
{
    /// <summary>
    /// Navigation target defined by a transform
    /// </summary>
    public class TransformNavigationTarget : INavigationTarget
    {
        readonly Transform _transform;
        readonly bool _specifyOrientation;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="transform">Transform that provides navigation target</param>
        /// <param name="specifyOrientation">True if transform orientation should be provided, false otherwise</param>
        public TransformNavigationTarget([NotNull] Transform transform, bool specifyOrientation)
        {
            if (transform == null)
                throw new ArgumentNullException(nameof(transform));

            _transform = transform;
            _specifyOrientation = specifyOrientation;
        }

        public void GetTarget(Vector3 currentPosition, float currentYaw, out Vector3 targetPosition, out float? targetYaw)
        {
            targetPosition = _transform.position;
            targetYaw = _specifyOrientation
                ? _transform.rotation.eulerAngles.y
                : null;
        }
    }
}