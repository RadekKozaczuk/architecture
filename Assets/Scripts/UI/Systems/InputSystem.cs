using Common.Enums;
using Common.Systems;
using Presentation.ViewModels;
using UI.Config;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Systems
{
    static class InputSystem
    {
        const string Quit = "Quit";
        const string MoveUp = "Move Up";
        const string MoveDown = "Move Down";
        const string MoveLeft = "Move Left";
        const string MoveRight = "Move Right";

        static readonly UIConfig _uiConfig;

        internal static void Initialize()
        {
            // MainMenu bindings
            InputActionMap mainMenu = _uiConfig.InputActionAsset.FindActionMap("MainMenu");
            mainMenu.FindAction(Quit).performed += _ => Application.Quit();

            // Gameplay bindings
            InputActionMap gameplay = _uiConfig.InputActionAsset.FindActionMap("Gameplay");
            gameplay.FindAction(Quit).performed += _ => GameStateSystem.RequestStateChange(GameState.MainMenu);
            gameplay.FindAction(MoveUp).performed += _ => PresentationViewModel.MoveUp();
            gameplay.FindAction(MoveDown).performed += _ => PresentationViewModel.MoveDown();
            gameplay.FindAction(MoveLeft).performed += _ => PresentationViewModel.MoveLeft();
            gameplay.FindAction(MoveRight).performed += _ => PresentationViewModel.MoveRight();
        }
    }
}