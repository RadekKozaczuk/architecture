#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable
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
        public readonly string LobbyCode;
        public readonly List<(string playerName, string playerId, bool isHost)> Players;

        public LobbyChangedSignal(string lobbyName, string lobbyCode, List<(string playerName, string playerId, bool isHost)> players)
        {
            LobbyName = lobbyName;
            LobbyCode = lobbyCode;
            Players = players;
        }
    }
}