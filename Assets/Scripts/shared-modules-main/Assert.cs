#nullable enable
using System;
using System.Diagnostics;

namespace Shared
{
    public static class Assert
    {
        /// <summary>
        /// Checks if the value is false. Otherwise throws an exception.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void False(bool value, string? msg = null)
        {
            if (value)
                throw new Exception(msg ?? "Assertion failed. Value not false.");
        }

        /// <summary>
        /// Checks if the value is true. Otherwise throws an exception.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void True(bool value, string? msg = null)
        {
            if (!value)
                throw new Exception(msg ?? "Assertion failed. Value not true.");
        }

        /// <summary>
        /// Checks if the object has a value. Otherwise throws an exception.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void IsNotNull(object value, string? msg = null)
        {
            if (value == null)
                throw new Exception(msg ?? "Assertion failed. Received null pointer when value was expected.");
        }

        /// <summary>
        /// Checks if the object is null. Otherwise throws an exception.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void IsNull(object value, string? msg = null)
        {
            if (value != null)
                throw new Exception(msg ?? "Assertion failed. Received an object when a null pointer was expected.");
        }
    }
}