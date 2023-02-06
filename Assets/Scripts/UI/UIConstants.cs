namespace UI
{
    static class UIConstants
    {
        // action maps
        internal const string MainMenuActionMap = "MainMenu";
        internal const string GameplayActionMap = "Gameplay";
        internal const string PopupActionMap = "Popup";

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        internal const string DebugCommandsMap = "DebugCommands";
#endif
    }
}