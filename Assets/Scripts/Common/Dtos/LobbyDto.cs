namespace Common.Dtos
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