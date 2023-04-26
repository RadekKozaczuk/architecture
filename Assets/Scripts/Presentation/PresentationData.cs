using System.Collections.Generic;
using Presentation.Views;

namespace Presentation
{
    /// <summary>
    /// Assembly-level data.
    /// </summary>
    static class PresentationData
    {
        internal static PlayerView Player;
        internal static readonly List<PlayerNetworkView> NetworkPlayers = new ();
    }
}