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
            _newGame.onClick.AddListener(() => GameStateSystem.RequestStateChange(GameState.Gameplay));
            _quit.onClick.AddListener(Application.Quit);
        }
    }
}