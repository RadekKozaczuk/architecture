using UI.Views;

namespace UI
{
    /// <summary>
    /// Assembly-level data.
    /// </summary>
    static class UIData
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        internal static DebugConsoleView DebugConsoleView;
        internal static DebugMobileConsoleView DebugMobileConsole;
#endif
    }
}