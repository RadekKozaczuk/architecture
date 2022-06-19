using Common.Enums;
using Common.Systems;
using GameLogic.Config;
using JetBrains.Annotations;
using Presentation.Controllers;
using Shared.SignalProcessing;
using Zenject;

namespace GameLogic.Controllers
{
    /// <summary>
    /// Main logic of the game.
    /// Additionally provide more convenient way to set up the order of execution.
    /// For better code readability all controllers meant to interact with this controller should implement
    /// <see cref="Shared.Interfaces.ICustomLateUpdate" /> interface.
    /// </summary>
    [UsedImplicitly]
    [ReactOnSignals]
    class GameLogicMainController
    {
        // TODO: convert to constructor injection for better performance
        [Inject]
        readonly PresentationMainController _presentationMainController;

        [Inject]
        readonly GameplayConfig _gameplayConfig;

#region Unity life-cycle methods
        [UsedImplicitly]
        public void Initialize()
        {
            SignalProcessor.AddReactiveController(this);
        }

        [UsedImplicitly]
        public void Tick()
        {
            if (GameStateSystem.CurrentState == GameState.Booting)
                return;

            GameStateSystem.CustomUpdate();
        }

        [UsedImplicitly]
        public void LateTick()
        {
            if (GameStateSystem.CurrentState == GameState.Booting)
                return;
        }
#endregion
    }
}