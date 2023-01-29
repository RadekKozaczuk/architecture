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

        QuitGamePopup() : base(PopupType.QuitGame) { }

        void Awake()
        {
            _saveGame.onClick.AddListener(SaveGameAction);
            _loadGame.onClick.AddListener(LoadGameAction);
            _hub.onClick.AddListener(HubAction);
            _mainMenu.onClick.AddListener(MainMenuAction);
        }

        internal override void Initialize() => _hub.interactable = CommonData.CurrentLevel != null;

        static void SaveGameAction()
        {
            GameLogicViewModel.SaveGame();
        }

        static void LoadGameAction()
        {
            GameLogicViewModel.LoadGame();
        }

        static void HubAction()
        {
            PopupSystem.CloseCurrentPopup();
        }

        static void MainMenuAction()
        {
            PopupSystem.CloseCurrentPopup();
            GameStateSystem.RequestStateChange(GameState.MainMenu);
        }
    }
}