using System;

namespace Boot
{
    static class ExtensionsMethods
    {
        /// <summary>
        ///     Value is trimmed and case insensitive.
        /// </summary>
        internal static bool Contains(this string[] array, string value)
            => Array.Find(array, s => string.Equals(s, value.Trim(), StringComparison.CurrentCultureIgnoreCase)) != null;
    }
}