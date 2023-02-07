using Common;
using Common.Enums;
using Common.Systems;
using GameLogic.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class MainMenuView : MonoBehaviour
    {
        [SerializeField]
        Button _newGame;

        [SerializeField]
        Button _loadGame;

        [SerializeField]
        Button _options;

        [SerializeField]
        Button _quit;

        void Awake()
        {
            _newGame.onClick.AddListener(NewGame);
            _loadGame.onClick.AddListener(LoadGame);
            _loadGame.interactable = GameLogicViewModel.SaveFileExist;
            _quit.onClick.AddListener(Application.Quit);
        }

        static void NewGame()
        {
            CommonData.CurrentLevel = Level.HubLocation;
            GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CommonData.CurrentLevel});
        }

        static void LoadGame()
        {
            CommonData.LoadRequested = true;
            GameStateSystem.RequestStateChange(GameState.Gameplay);
        }
    }
}