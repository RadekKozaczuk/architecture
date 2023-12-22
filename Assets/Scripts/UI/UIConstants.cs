#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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