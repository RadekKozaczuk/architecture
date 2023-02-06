using Common.Enums;
using Presentation.ViewModels;
using UI.Config;
using UI.Popups;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Systems
{
    static class InputSystem
    {
        const string Quit = "Quit";
        const string Movement = "Movement";

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        const string ToggleConsole = "ToggleConsole";
        const string TakeBestMatch = "TakeBestMatch";
#endif

        static readonly UIConfig _uiConfig;

        static InputAction _movementAction;

        /// <summary>
        /// True if movement action is pressed down.
        /// </summary>
        static bool _movementDown;

        internal static void Initialize()
        {
            // MainMenu bindings
            InputActionMap mainMenu = _uiConfig.InputActionAsset.FindActionMap(UIConstants.MainMenuActionMap);
            mainMenu.FindAction(Quit).performed += _ => Application.Quit();

            // Gameplay bindings
            InputActionMap gameplay = _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap);
            gameplay.FindAction(Quit).performed += _ => PopupSystem.ShowPopup(PopupType.QuitGame);
            _movementAction = gameplay.FindAction(Movement);
            _movementAction.performed += _ => _movementDown = true;
            _movementAction.canceled += _ => _movementDown = false;

            // Popups bindings
            InputActionMap popup = _uiConfig.InputActionAsset.FindActionMap(UIConstants.PopupActionMap);
            popup.FindAction(Quit).performed += _ => PopupSystem.CloseCurrentPopup();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // DebugCommands bindings
            InputActionMap debugCommands = _uiConfig.InputActionAsset.FindActionMap(UIConstants.DebugCommandsMap);
            debugCommands.FindAction(ToggleConsole).performed += _ => UIData.DebugConsoleView.ToggleConsole();
            debugCommands.FindAction(TakeBestMatch).performed += _ => UIData.DebugConsoleView.TakeBestMatchAsCommand();
#endif
        }

        internal static void CustomUpdate()
        {
            if (_movementDown)
                PresentationViewModel.Movement(_movementAction.ReadValue<Vector2>());
        }
    }
}