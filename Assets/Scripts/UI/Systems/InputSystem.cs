using Common.Enums;
using Common.Systems;
using Presentation.ViewModels;
using UI.Config;
using UI.Popups;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Systems
{
    static class InputSystem
    {
        const string MainMenuActionMap = "MainMenu";
        const string GameplayActionMap = "Gameplay";

        const string Quit = "Quit";
        const string Movement = "Movement";

        static readonly UIConfig _uiConfig;

        static InputAction _movementAction;

        /// <summary>
        /// True if movement action is pressed down.
        /// </summary>
        static bool _movementDown;

        internal static void Initialize()
        {
            // MainMenu bindings
            InputActionMap mainMenu = _uiConfig.InputActionAsset.FindActionMap(MainMenuActionMap);
            mainMenu.FindAction(Quit).performed += _ => Application.Quit();

            // Gameplay bindings
            InputActionMap gameplay = _uiConfig.InputActionAsset.FindActionMap(GameplayActionMap);
            gameplay.FindAction(Quit).performed += _ => QuitAction();

            // Popups bindings
            _movementAction = gameplay.FindAction(Movement);
            _movementAction.performed += _ => _movementDown = true;
            _movementAction.canceled += _ => _movementDown = false;
        }

        internal static void CustomUpdate()
        {
            if (_movementDown)
                PresentationViewModel.Movement(_movementAction.ReadValue<Vector2>());
        }

        static void QuitAction()
        {
            if (PopupSystem.CurrentPopup == null)
                PopupSystem.ShowPopup(PopupType.QuitGame);
            else
                PopupSystem.CloseCurrentPopup();
        }
    }
}