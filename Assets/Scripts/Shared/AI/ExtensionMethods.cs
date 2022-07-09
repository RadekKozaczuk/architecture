using System;
using JetBrains.Annotations;

namespace Shared.AI
{
    static class ExtensionMethods
    {
        /// <summary>
        /// Returns current controller yaw
        /// </summary>
        internal static float GetCurrentYaw([NotNull] this NavMeshNavigationController self)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return self.CurrentRotation.eulerAngles.y;
        }
    }
}