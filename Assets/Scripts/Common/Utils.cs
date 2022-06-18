using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Common
{
    public static class Utils
    {
        public static bool SolveQuadraticEquation(float a, float b, float c, out float root1, out float root2)
        {
            Assert.False(a == 0, "Not a quadratic equation, 'a' cannot be 0 for quadratics.");

            float delta = b * b - 4 * a * c;
            switch (delta)
            {
                case > 0:
                    root1 = (-b + (float) Math.Sqrt(delta)) / (2 * a);
                    root2 = (-b - (float) Math.Sqrt(delta)) / (2 * a);
                    return true;
                case 0:
                    root1 = root2 = -b / (2 * a);
                    return true;
            }

            root1 = int.MinValue;
            root2 = int.MinValue;
            return false;
        }

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
        public static float Map(float newMin, float newMax, float oldMin, float oldMax, float value)
            => Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(oldMin, oldMax, value));
    }
}