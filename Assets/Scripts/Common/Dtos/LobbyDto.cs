namespace Common.Dtos
{
    public struct LobbyDto
    {
        public string LobbyId;
        public string LobbyName;
        public int PlayerCount;
        public int PlayerMax;

        public LobbyDto(string lobbyId, string lobbyName, int playerCount, int playerMax)
        {
            LobbyId = lobbyId;
            LobbyName = lobbyName;
            PlayerCount = playerCount;
            PlayerMax = playerMax;
        }
    }
}