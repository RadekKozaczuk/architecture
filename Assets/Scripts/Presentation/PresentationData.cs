#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable
using Presentation.Views;

namespace Presentation
{
    /// <summary>
    /// Assembly-level data.
    /// </summary>
    static class PresentationData
    {
        /// <summary>
        /// This will contain a reference only when in a single player mode.
        /// </summary>
        internal static PlayerView Player;

        /// <summary>
        /// This will contain all the player references on the server and only the client's player on a client.
        /// </summary>
        internal static readonly PlayerNetworkView[] NetworkPlayers = new PlayerNetworkView[4];

        /// <summary>
        /// When we host a game, other players are spawn in their respective spawn points as single-player prefabs.
        /// As soon as clients join the single-player object is destroyed and immediately replaced by a multi-player prefab.
        /// </summary>
        internal static readonly PlayerView[] InstantiatedSpPlayers = new PlayerView[4];
    }
}