using System.Collections.Generic;
using Shared;

namespace Common.Signals
{
    /// <summary>
    /// Indicates that something has changed in the lobby.
    /// If LobbyName is different than null then the name changed.
    /// If Players list is different than null then something changed in one or more players.
    /// The list always represents the current state, and not only the change.
    /// </summary>
    public sealed class LobbyChangedSignal : AbstractSignal
    {
        public readonly string LobbyName;
        public readonly List<(string playerName, string playerId, bool isHost)> Players;

        public LobbyChangedSignal(string lobbyName, List<(string playerName, string playerId, bool isHost)> players)
        {
            LobbyName = lobbyName;
            Players = players;
        }
    }
}