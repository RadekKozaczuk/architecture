namespace Common
{
    /// <summary>
    /// Application-level (global) data.
    /// All static data objects used across the project are stored here.
    /// </summary>
    public static class CommonData
    {
        /// <summary>
        /// Used to tell <see cref="Systems.GameStateSystem"/> that next transition from <see cref="Common.Enums.GameState.MainMenu"/>
        /// to <see cref="Common.Enums.GameState.Gameplay"/> will include save game loading.
        /// </summary>
        public static bool LoadRequested;

        /// <summary>
        /// Indicates if player is in the HubLocation.
        /// Setting it to true will automatically set <see cref="CurrentLevel"/> to null.
        /// </summary>
        public static bool IsInHubLocation
        {
            get => _isInHubLocation;
            set
            {
                if (value)
                {
                    _isInHubLocation = true;
                    _currentLevel = null;
                }
                else
                {
                    _isInHubLocation = false;
                }
            }
        }
        static bool _isInHubLocation = true;

        /// <summary>
        /// Indicates if player is in a level, and if so tells the level id.
        /// Setting this value will automatically set <see cref="IsInHubLocation"/>.
        /// </summary>
        public static int? CurrentLevel
        {
            get => _currentLevel;
            set
            {
                _currentLevel = value;
                _isInHubLocation = value == null;
            }
        }
        static int? _currentLevel;
    }
}