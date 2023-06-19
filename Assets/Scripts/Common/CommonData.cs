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

        /// <summary>
        /// We cannot use <see cref="NetworkManager.IsServer"/> as that field can be true together with <see cref="NetworkManager.IsClient"/>
        /// when the game is run on a host.
        /// Our IsServer will be true only when the given instance is hosting or running the server.
        /// </summary>
        public static bool IsServer;

        public static Level CurrentLevel;

        /// <summary>
        /// Selected player. Useful only in multiplayer.
        /// </summary>
        public static PlayerId? PlayerId;

        public static BinaryReader SaveGameReader;

        /// <summary>
        /// Used only in multiplayer context.
        /// </summary>
        public static string PlayerName;
    }
}