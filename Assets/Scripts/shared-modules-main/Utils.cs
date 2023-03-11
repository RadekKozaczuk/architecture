using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Shared
{
    public static class Utils
    {
        public static async Task<bool> WaitForNextFrame()
        {
            int currentFrameCount = Time.frameCount;

            while (currentFrameCount == Time.frameCount)
                await Task.Delay(1);

            return true;
        }

        /// <summary>
        /// Takes rotation along Y axis in degrees (0 means north), returns normalized direction vector.
        /// </summary>
        public static Vector3 ConvertRotationToDirectionVector(float degrees) =>
            new Vector3(Mathf.Sin(degrees * Mathf.Deg2Rad), 0, Mathf.Cos(degrees * Mathf.Deg2Rad)).normalized;

        /// <summary>
        /// Returns normalized direction vector.
        /// </summary>
        public static Vector3 ConvertRotationToDirectionVector(Quaternion rotation) => (rotation * Vector3.forward).normalized;

        /// <summary>
        /// Maps value from one range to another.
        /// </summary>
        public static float Map(float newMin, float newMax, float oldMin, float oldMax, float value) =>
            Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(oldMin, oldMax, value));

        public static bool HasDuplicates(int[] array)
        {
            var set = new HashSet<int>();
            for (int i = 0; i < array.Length; i++)
                if (!set.Add(array[i]))
                    return true;

            return false;
        }
    }
}