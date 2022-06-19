using System.Diagnostics;

namespace Shared
{
    public static class MyDebug
    {
        /// <summary>
        /// This is just a normal Debug.Log except it compiles conditionally and exists only in UNITY_EDITOR and DEVELOPMENT_BUILD builds.
        /// Use it whenever you want to save 2 lines of code and make your code cleaner.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string msg) => UnityEngine.Debug.Log(msg);

        /// <summary>
        /// This is just a normal Debug.Log except it compiles conditionally and exists only in UNITY_EDITOR and DEVELOPMENT_BUILD builds.
        /// Use it whenever you want to save 2 lines of code and make your code cleaner.
        /// Additional condition parameter allows for another line save on if-statement.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string msg, bool condition)
        {
            if (condition)
                UnityEngine.Debug.Log(msg);
        }
    }
}