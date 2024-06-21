#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using Core.Enums;
using Core.Systems;
using GameLogic.ViewModels;
using Presentation.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class QuitGamePopup : AbstractPopup
    {
        [SerializeField]
        Button _saveGame;

        [SerializeField]
        Button _loadGame;

        [SerializeField]
        Button _hub;

        [SerializeField]
        Button _mainMenu;

        QuitGamePopup()
            : base(PopupType.QuitGame) { }

        internal override void Initialize()
        {
            base.Initialize();

            _hub.interactable = CommonData.CurrentLevel != Level.HubLocation;

            _saveGame.onClick.AddListener(SaveGameAction);
            _loadGame.onClick.AddListener(LoadGameAction);
            _loadGame.interactable = GameLogicViewModel.SaveFileExist;
            _hub.onClick.AddListener(HubAction);
            _mainMenu.onClick.AddListener(MainMenuAction);
        }

        void SaveGameAction()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            GameLogicViewModel.SaveGame();
            _loadGame.interactable = GameLogicViewModel.SaveFileExist;
        }

        static void LoadGameAction()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            PopupSystem.CloseCurrentPopup();
            GameStateSystem.RequestStateChange(GameState.Gameplay,
                                               parameters: new []{(StateTransitionParameter.LoadGameRequested, (object)true)});
        }

        static void HubAction()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            PopupSystem.CloseCurrentPopup();
            GameStateSystem.RequestStateChange(GameState.Gameplay,
                                               parameters: new []{(StateTransitionParameter.HubSceneRequested, (object)true)});
        }

        static void MainMenuAction()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            PopupSystem.CloseCurrentPopup();
            GameStateSystem.RequestStateChange(GameState.MainMenu);
        }
    }
}