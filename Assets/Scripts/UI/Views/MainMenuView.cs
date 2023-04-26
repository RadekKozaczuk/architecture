using Common;
using Common.Enums;
using Common.Systems;
using GameLogic.ViewModels;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI.Views
{
    [DisallowMultipleComponent]
    class MainMenuView : MonoBehaviour
    {
        [SerializeField]
        Button _newGame;

        [SerializeField]
        Button _coop;

        [SerializeField]
        Button _loadGame;

        [SerializeField]
        Button _options;

        [SerializeField]
        Button _quit;

        void Awake()
        {
            _newGame.onClick.AddListener(NewGame);
            _coop.onClick.AddListener(Coop);
            _loadGame.onClick.AddListener(LoadGame);
            _loadGame.interactable = GameLogicViewModel.SaveFileExist;
            _quit.onClick.AddListener(Quit);
        }

        static void NewGame()
        {
            CommonData.CurrentLevel = Level.HubLocation;
            GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CommonData.CurrentLevel});
        }

        static void Coop()
        {
            CommonData.CurrentLevel = Level.HubLocation;
            GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CommonData.CurrentLevel});
            CommonData.IsMultiplayer = true;
            CommonData.IsServer = true;
            NetworkManager.Singleton.StartHost();
        }

        static void LoadGame()
        {
            CommonData.LoadRequested = true;
            GameStateSystem.RequestStateChange(GameState.Gameplay);
        }

        static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}