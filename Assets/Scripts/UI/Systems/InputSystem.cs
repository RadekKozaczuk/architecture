using Common.Enums;
using Common.Systems;
using UI.Config;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Systems
{
    static class InputSystem
    {
        static readonly UIConfig _uiConfig;

        internal static void Initialize()
        {
            // MainMenu bindings
            InputActionMap mainMenu = _uiConfig.InputActionAsset.FindActionMap("MainMenu");
            mainMenu.FindAction("Quit").performed += _ => Application.Quit();

            // Gameplay bindings
            InputActionMap gameplay = _uiConfig.InputActionAsset.FindActionMap("Gameplay");
            gameplay.FindAction("Quit").performed += _ => GameStateSystem.RequestStateChange(GameState.MainMenu);
        }
    }
}