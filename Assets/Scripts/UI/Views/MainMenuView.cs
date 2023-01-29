using Common;
using Common.Enums;
using Common.Systems;
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
            _quit.onClick.AddListener(Application.Quit);
        }

        static void NewGame()
        {
            int sceneId = CommonData.CurrentLevel.HasValue ? Constants.Level0Scene : Constants.HubScene;
            GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {sceneId});
        }

        static void LoadGame()
        {
            CommonData.LoadRequested = true;
            GameStateSystem.RequestStateChange(GameState.Gameplay);
        }
    }
}