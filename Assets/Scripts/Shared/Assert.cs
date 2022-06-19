using System;
using System.Diagnostics;

namespace Shared
{
    public static class Assert
    {
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void False(bool value, string msg = null)
        {
            if (value)
                throw new Exception(msg);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void True(bool value, string msg = null)
        {
            if (!value)
                throw new Exception(msg);
        }
    }
}