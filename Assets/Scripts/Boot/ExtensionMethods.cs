#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable
using System;

namespace Boot
{
    static class ExtensionMethods
    {
        /// <summary>
        /// Value is trimmed and case insensitive.
        /// </summary>
        internal static bool Contains(this string[] array, string value) =>
            Array.Find(array, s => string.Equals(s, value.Trim(), StringComparison.CurrentCultureIgnoreCase)) != null;
    }
}