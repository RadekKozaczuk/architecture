﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using ControlFlow.DependencyInjector;
using JetBrains.Annotations;
using UI.Config;
using UI.Controllers;
using UI.Popups;
using UI.Systems;
using UnityEngine.Scripting;

namespace UI.ViewModels
{
    [UsedImplicitly]
    public class UIViewModel : IInitializable
    {
        static readonly UIConfig _uiConfig;

        [Inject]
        static readonly UIMainController _uiMainController;

        [Preserve]
        UIViewModel() { }

        public void Initialize() => InputSystem.Initialize();

        public static void CustomFixedUpdate() => _uiMainController.CustomFixedUpdate();

        public static void CustomUpdate() => _uiMainController.CustomUpdate();

        public static void CustomLateUpdate() => _uiMainController.CustomLateUpdate();

        public static void OnUISceneLoaded()
        {
            UIMainController.OnUISceneLoaded();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _uiConfig.InputActionAsset.FindActionMap(UIConstants.DebugCommandsMap).Enable();
            Shared.DebugCommands.DebugCommandSystem.CanvasFunction = () => UISceneReferenceHolder.Canvas;
            Shared.DebugCommands.DebugCommandSystem.OnUISceneLoaded();
#endif
        }

        public static void BootingOnExit() { }

        public static void MainMenuOnEntry() => _uiConfig.InputActionAsset.FindActionMap(UIConstants.MainMenuActionMap).Enable();

        public static void MainMenuOnExit() => _uiConfig.InputActionAsset.FindActionMap(UIConstants.MainMenuActionMap).Disable();

        public static void GameplayOnEntry()
        {
            _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap).Enable();

            // this happens only when we start a client game starts
            if (PopupSystem.CurrentPopup != null)
                PopupSystem.CloseCurrentPopup();
        }

        public static void GameplayOnExit() => _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap).Disable();
    }
}