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
        /// Has game been run in multiplayer mode.
        /// </summary>
        public static bool IsMultiplayer;

        // todo: cant be true if is Multiplayer is false
        public static bool IsClient;

        public static bool IsServer;

        public static Level CurrentLevel;

        public static BinaryReader SaveGameReader;
    }
}