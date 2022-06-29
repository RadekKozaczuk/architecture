using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Shared.DependencyInjector.Internal;
using Shared.DependencyInjector.Util;

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
        
        public static void IsNotNull(object val)
        {
            if (val == null)
                throw new Exception("Assert Hit! Found null pointer when value was expected");
        }
    }
}