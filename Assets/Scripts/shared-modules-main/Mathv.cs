using UnityEngine;

namespace Shared
{
    // ReSharper disable once IdentifierTypo
    public static class Mathv
    {
        /// <summary>
        /// y is equal 0
        /// </summary>
        public static Vector3 GetVector3FromAngle(float angle)
        {
            float angleRad = angle * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
        }

        public static Vector2 GetVector2FromAngle(float angle)
        {
            float angleRad = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }
    }
}