namespace Common.Enums
{
    public enum GameState
    {
        Booting,
        MainMenu,
        Gameplay
    }

    public enum Sound
    {
        ArtilleryShotHeavy,
        ExplosionInTightSpace,
        ExplosionNearby,
        ExplosionWithRubbleDebris
    }

    public enum Level
    {
        HubLocation = 4,
        Level0 = 5
    }

    public enum Music
    {
        MainMenu
    }

    public enum VFX
    {
        HitEffect
    }

    public enum PopupType
    {
        QuitGame,
        /// <summary>
        /// List of lobbies.
        /// </summary>
        LobbyList,
        CreateLobby,
        /// <summary>
        /// The lobby you joined/created.
        /// </summary>
        Lobby
    }

    public enum PlayerId
    {
        Player1,
        Player2,
        Player3,
        Player4
    }
}