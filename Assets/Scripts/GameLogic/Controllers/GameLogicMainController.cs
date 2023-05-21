using Common.Enums;
using Common.Systems;
using ControlFlow.Interfaces;
using GameLogic.Systems;
using JetBrains.Annotations;
using Shared.Systems;
using UnityEngine.Scripting;

namespace GameLogic.Controllers
{
    /// <summary>
    /// Main controller serves 3 distinct roles:
    /// 1) It allows you to order the signal execution order. So instead of reacting separately in two different controllers you can react in main controller
    /// and call adequate methods.
    /// 2) Serves as a 'default' controller. When you don't know where to put some logic or the logic is too small for its own controller you can put it into
    /// the main controller.
    /// 3) Reduces the size of the view model. We could move all (late/fixed)update calls to view model but as the application grows it would lead to view
    /// model doing to much stuff.
    /// For better code readability all controllers meant to interact with this controller should implement
    /// <see cref="ICustomLateUpdate" /> interface.
    /// </summary>
    [UsedImplicitly]
    class GameLogicMainController : ICustomFixedUpdate, ICustomUpdate, ICustomLateUpdate
    {
        [Preserve]
        GameLogicMainController() { }

        public void CustomFixedUpdate() { }

        public void CustomUpdate()
        {
            if (GameStateSystem.CurrentState == GameState.Booting)
                return;

            LobbySystem.CustomUpdate();
        }

        public void CustomLateUpdate()
        {
            if (GameStateSystem.CurrentState == GameState.Booting)
                return;

            SignalProcessor.ExecuteSentSignals();
        }
	}
}