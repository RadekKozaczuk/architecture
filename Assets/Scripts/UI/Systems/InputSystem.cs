#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Common.Enums;
using GameLogic.ViewModels;
using Presentation.ViewModels;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Shared.DebugCommands;
#endif
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
        const string JoystickMovement = "JoystickMovement";

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
            mainMenu.FindAction(Quit).performed += _ =>
            {
                // if there is a popup - close it
                // otherwise quit the game
                if (PopupSystem.CurrentPopup == null)
                {
                    GameLogicViewModel.QuitGame();

#if UNITY_EDITOR
                    UnityEditor.EditorApplication.ExitPlaymode();
#else
                    Application.Quit();
#endif
                }
                else
                    PopupSystem.CloseCurrentPopup();
            };

            // Gameplay bindings
            InputActionMap gameplay = _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap);
            gameplay.FindAction(Quit).performed += _ => PopupSystem.ShowPopup(PopupType.QuitGame);

#if PLATFORM_ANDROID || PLATFORM_IOS
            _movementAction = gameplay.FindAction(JoystickMovement);
#else
            _movementAction = gameplay.FindAction(Movement);
#endif
            _movementAction.performed += _ => _movementDown = true;
            _movementAction.canceled += _ => _movementDown = false;

            // Popups bindings
            InputActionMap popup = _uiConfig.InputActionAsset.FindActionMap(UIConstants.PopupActionMap);
            popup.FindAction(Quit).performed += _ => PopupSystem.CloseCurrentPopup();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // DebugCommands bindings
            InputActionMap debugCommands = _uiConfig.InputActionAsset.FindActionMap(UIConstants.DebugCommandsMap);
            debugCommands.FindAction(ToggleConsole).performed += _ =>
            {
                InputActionMap gamePlayActionMap = _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap);

                if (DebugCommandSystem.IsConsoleOpen)
                {
                    DebugCommandSystem.CloseConsole();
                    gamePlayActionMap.Enable();
                }
                else
                {
                    DebugCommandSystem.OpenConsole();
                    gamePlayActionMap.Disable();
                }
            };
            debugCommands.FindAction(TakeBestMatch).performed += _ => DebugCommandSystem.TakeBestMatchAsCommand();
#endif
        }

        internal static void CustomUpdate()
        {
            if (_movementDown)
                PresentationViewModel.Movement(_movementAction.ReadValue<Vector2>());
        }
    }
}