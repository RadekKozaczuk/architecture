#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using ControlFlow.Interfaces;
using JetBrains.Annotations;
using Shared;
using UI.Popups;
using UI.Popups.Views;
using UI.Systems;
using UnityEngine.Scripting;

namespace UI.Controllers
{
    /// <summary>
    /// Main controller serves 3 distinct roles:
    /// 1) It allows you to control signal execution order. For example, instead of reacting on many signals in many different controllers,
    /// you can have one signal, react on it here, and call necessary controllers/systems in the order of your liking.
    /// 2) Serves as a 'default' controller. When you don't know where to put some logic or the logic is too small for its own controller
    /// you can put it into the main controller.
    /// 3) Reduces the size of the viewmodel. We could move all (late/fixed)update calls to viewmodel but over time it would lead to viewmodel
    /// being too long to comprehend. We also do not want to react on signals in viewmodels for the exact same reason.
    /// For better code readability all controllers meant to interact with this controller should implement
    /// <see cref="ICustomLateUpdate" /> interface.
    /// </summary>
    [UsedImplicitly]
    class UIMainController : ICustomFixedUpdate, ICustomUpdate, ICustomLateUpdate
    {
        static bool _uiSceneLoaded;

        [Preserve]
        UIMainController() { }

        public void CustomUpdate()
        {
            if (!_uiSceneLoaded)
                return;

            InputSystem.CustomUpdate();
        }

        public void CustomFixedUpdate() { }

        public void CustomLateUpdate() { }

        internal static void OnUISceneLoaded() => _uiSceneLoaded = true;

        [React]
        static void OnInventoryChanged() { }

        [React]
        static void OnLobbyChanged(string lobbyName, string lobbyCode, List<(string playerName, string playerId, bool isHost)> players) =>
            (PopupSystem.CurrentPopup as LobbyPopup)?.UpdateLobby(lobbyName, lobbyCode, players);

        [React]
        static void OnMissionComplete() { }

        [React]
        static void OnMissionFailed() { }
    }
}