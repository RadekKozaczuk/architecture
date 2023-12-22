#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable
using Common;
using Common.Enums;
using Common.Systems;
using GameLogic.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class QuitGamePopup : AbstractPopupView
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

        void Awake()
        {
            _saveGame.onClick.AddListener(SaveGameAction);
            _loadGame.onClick.AddListener(LoadGameAction);
            _loadGame.interactable = GameLogicViewModel.SaveFileExist;
            _hub.onClick.AddListener(HubAction);
            _mainMenu.onClick.AddListener(MainMenuAction);
        }

        internal override void Initialize() => _hub.interactable = CommonData.CurrentLevel != Level.HubLocation;

        void SaveGameAction()
        {
            GameLogicViewModel.SaveGame();
            _loadGame.interactable = GameLogicViewModel.SaveFileExist;
        }

        static void LoadGameAction()
        {
            PopupSystem.CloseCurrentPopup();
            GameStateSystem.RequestStateChange(GameState.Gameplay,
                                               parameters: new []{(StateTransitionParameter.LoadGameRequested, (object)true)});
        }

        static void HubAction()
        {
            PopupSystem.CloseCurrentPopup();
            GameStateSystem.RequestStateChange(GameState.Gameplay,
                                               parameters: new []{(StateTransitionParameter.HubSceneRequested, (object)true)});
        }

        static void MainMenuAction()
        {
            PopupSystem.CloseCurrentPopup();
            GameStateSystem.RequestStateChange(GameState.MainMenu);
        }
    }
}