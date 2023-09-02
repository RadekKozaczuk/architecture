using System.IO;
using Common.Enums;

namespace Common
{
    /// <summary>
    /// Application-level (global) data.
    /// All static data objects used across the project are stored here.
    /// </summary>
    public static class CommonData
    {
        /// <summary>
        /// Used to tell <see cref="Systems.GameStateSystem" /> that next transition from <see cref="Common.Enums.GameState.MainMenu" />
        /// to <see cref="Common.Enums.GameState.Gameplay" /> will include save game loading.
        /// </summary>
        public static bool LoadRequested;

        /// <summary>
        /// Used to tell <see cref="Systems.GameStateSystem" /> that during next transition from <see cref="Common.Enums.GameState.Gameplay" />
        /// to <see cref="Common.Enums.GameState.Gameplay" /> player will go to <see cref="Level.HubLocation"/>.
        /// </summary>
        public static bool HubLocationRequested;

        /// <summary>
        /// Has game been run in multiplayer mode.
        /// </summary>
        public static bool IsMultiplayer;

        public static Level CurrentLevel;

        /// <summary>
        /// Selected player. Useful only in multiplayer.
        /// </summary>
        public static PlayerId? PlayerId;

        public static BinaryReader SaveGameReader;

        /// <summary>
        /// This is only used in multiplayer (always null in single player).
        /// When someone starts the game this value is set to the player count.
        /// </summary>
        public static int? PlayerCount;

        /// <summary>
        /// Used only in multiplayer context.
        /// </summary>
        public static string PlayerName;
    }
}