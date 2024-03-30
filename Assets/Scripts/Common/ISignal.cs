using System.Collections.Generic;
// ReSharper disable UnusedParameter.Global

namespace Common
{
    public interface ISignal
    {
        /// <summary>
        /// Universal signal indicating that something has changed in the inventory.
        /// An item was added, removed, moved to a different slot, or its quantity has changed.
        /// </summary>
        void InventoryChanged();

        /// <summary>
        /// Indicates that something has changed in the lobby.
        /// If LobbyName is different than null then the name changed.
        /// If Players list is different than null then something changed in one or more players.
        /// The list always represents the current state, and not only the change.
        /// </summary>
        void LobbyChanged(string lobbyName, string lobbyCode, List<(string playerName, string playerId, bool isHost)> players);

        void MissionComplete();
        void MissionFailed();
    }
}