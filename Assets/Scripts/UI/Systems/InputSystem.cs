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
            InputActionMap inputActionMap = _uiConfig.InputActionAsset.FindActionMap("Player");

            // Buttons
            inputActionMap.FindAction("Quit/Return").performed += context =>
            {
                if (GameStateSystem.CurrentState == GameState.Gameplay)
                    GameStateSystem.RequestStateChange(GameState.MainMenu);
                else if (GameStateSystem.CurrentState == GameState.MainMenu)
                    Application.Quit();
            };
        }
    }
}