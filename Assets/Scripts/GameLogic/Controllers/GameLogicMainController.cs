using Common.Enums;
using Common.Signals;
using Common.Systems;
using GameLogic.Config;
using JetBrains.Annotations;
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
    class GameLogicMainController : ICustomFixedUpdate, ICustomUpdate, ICustomLateUpdate
    {
        static readonly GameplayConfig _gameplayConfig;

        [Preserve]
        GameLogicMainController() { }

        public void CustomFixedUpdate() { }

        public void CustomUpdate()
        {
            if (GameStateSystem.CurrentState == GameState.Booting)
                return;
        }

        public void CustomLateUpdate()
        {
            if (GameStateSystem.CurrentState == GameState.Booting)
                return;
        }

        [React]
        void OnMissionFailed(MissionFailedSignal _) { }
    }
}