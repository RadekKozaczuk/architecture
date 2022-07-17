using Common.Enums;
using Common.Signals;
using Common.Systems;
using GameLogic.Config;
using JetBrains.Annotations;
using Presentation.Controllers;
using Shared.DependencyInjector;
using Shared.DependencyInjector.Attributes;
using Shared.DependencyInjector.Interfaces;
using Shared.Interfaces;
using Shared.SignalProcessing;
using UnityEngine.Scripting;

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
    class GameLogicMainController : IInitializable, ICustomUpdate, ICustomFixedUpdate, ICustomLateUpdate
    {
        // TODO: convert to constructor injection for better performance
        [Inject]
        readonly PresentationMainController _presentationMainController;

        static readonly GameplayConfig _gameplayConfig;

        [Preserve]
        GameLogicMainController() { }

        [UsedImplicitly]
        public void Initialize()
        {
            SignalProcessor.AddReactiveController(this);  
        } 

        public void CustomUpdate()
        {
            if (GameStateSystem.CurrentState == GameState.Booting)
                return;
        }

        public void CustomFixedUpdate() { }

        public void CustomLateUpdate()
        {
            if (GameStateSystem.CurrentState == GameState.Booting)
                return;
        }

        [React]
        void OnMissionFailed(MissionFailedSignal _) { }
    }
}