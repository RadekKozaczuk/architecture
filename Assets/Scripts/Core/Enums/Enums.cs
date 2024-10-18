#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Core.Enums
{
    public enum GameState
    {
        Booting,
        MainMenu,
        Gameplay
    }

    public enum StateTransitionParameter
    {
        /// <summary>
        /// This transition goes to HubLocation whether from MainMenu or a Level.
        /// </summary>
        HubSceneRequested,
        LoadGameRequested
    }

    public enum Sound
    {
        ClickSelect,
        Hit
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
        SigningIn,
        /// <summary>
        /// List of lobbies.
        /// </summary>
        LobbyList,
        ReconnectToLobby,
        CreateLobby,
        /// <summary>
        /// The lobby you joined/created.
        /// </summary>
        Lobby,
        /// <summary>
        /// Settings accessible from the main menu.
        /// </summary>
        Settings,
        /// <summary>
        /// Server Browser.
        /// </summary>
        ServerList,
        CreateServer
    }

    /// <summary>
    /// Tells what is the role of this machine (computer).
    /// </summary>
    public enum MachineRole
    {
        /// <summary>
        /// This machine does not know yet what it is going to do (f.e. is in the main menu).
        /// Or the value was not set which is an invalid state.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// This machine serves as a server as well as plays one player at the same time.
        /// Whether it means the game runs in editor or a build, on the same machine, or over Relay does not matter.
        /// </summary>
        Host = 1,
        /// <summary>
        /// This machine is a client and is connected to a server.
        /// Client only create Client world and runs Client systems.
        /// </summary>
        Client = 2,
        /// <summary>
        /// This machine serves a role of a dedicated server meaning it runs a version of the game stripped down of assets and presentation logic.
        /// Dedicated Server will only run Server systems and will only create Server world.
        /// However, the Server world in this case will be a special version of the world capable of running multiple simulations within itself.
        /// </summary>
        DedicatedServer = 3,
        /// <summary>
        /// This machine runs in a single player mode, and it does not use ghost synchronization.
        /// Machines running this mode will use different set of prefabs as well as different World and Systems than Client or Server.
        /// </summary>
        LocalSimulation = 4
    }
}