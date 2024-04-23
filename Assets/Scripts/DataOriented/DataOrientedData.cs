#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Unity.Mathematics;
using UnityEngine.Scripting;

[assembly: Preserve]
namespace DataOriented
{
    /// <summary>
    /// Assembly-level data. Everything here should be internal.
    /// </summary>
    static class DataOrientedData
    {
        /// <summary>
        /// This should be final value (meaning including player, delta time, and others).
        /// </summary>
        internal static float3 MoveRequest;

        internal static bool SpawnPlayer;
    }
}