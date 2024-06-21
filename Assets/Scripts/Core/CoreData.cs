#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.IO;
using Core.Enums;

namespace Core
{
    /// <summary>
    /// Application-level (global) data.
    /// All static data objects used across the project are stored here.
    /// </summary>
    public static class CommonData
    {
        /// <summary>
        /// Has game been run in multiplayer mode.
        /// </summary>
        public static bool IsMultiplayer;

        public static Level CurrentLevel;

        /// <summary>
        /// Selected player. Useful only in multiplayer.
        /// </summary>
        public static PlayerId? PlayerId;

        public static BinaryReader? SaveGameReader;

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