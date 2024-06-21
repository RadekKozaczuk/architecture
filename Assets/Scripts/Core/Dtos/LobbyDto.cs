#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Core.Dtos
{
    public struct LobbyDto
    {
        public readonly string LobbyId;
        public readonly string LobbyName;
        public readonly int PlayerCount;
        public readonly int PlayerMax;

        public LobbyDto(string lobbyId, string lobbyName, int playerCount, int playerMax)
        {
            LobbyId = lobbyId;
            LobbyName = lobbyName;
            PlayerCount = playerCount;
            PlayerMax = playerMax;
        }
    }
}