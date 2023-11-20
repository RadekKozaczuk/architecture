using Unity.Mathematics;

namespace Dots
{
    /// <summary>
    /// Assembly-level data. Everything here should be internal.
    /// </summary>
    static class DotsData
    {
        /// <summary>
        /// This should be final value (meaning including player, delta time, and others).
        /// </summary>
        internal static float3 MoveRequest;

        internal static bool SpawnPlayer;
    }
}