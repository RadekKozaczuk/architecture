using System.Collections.Generic;
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
        internal static readonly List<PlayerNetworkView> NetworkPlayers = new ();
    }
}