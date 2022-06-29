using System;
using JetBrains.Annotations;
using Shared.AI.Interfaces;

namespace Shared.AI
{
    public static class NavigationControllerExtensions
    {
        /// <summary>
        /// Returns current controller yaw
        /// </summary>
        public static float GetCurrentYaw([NotNull] this NavMeshNavigationController self)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return self.CurrentRotation.eulerAngles.y;
        }
    }
}