﻿using System.Collections.Generic;
// ReSharper disable UnusedMemberInSuper.Global

namespace Core
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
        /// If LobbyName is different from null then the name changed.
        /// If Players list is different from null then something changed in one or more players.
        /// The list always represents the current state, and not only the change.
        /// </summary>
        void LobbyChanged(string lobbyName, string lobbyCode, List<(string playerName, string playerId, bool isHost)> players);

        void MissionComplete();
        void MissionFailed();
        void ToggleMuteVoiceChat();
    }
}