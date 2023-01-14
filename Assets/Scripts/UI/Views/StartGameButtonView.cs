using Common.Enums;
using Common.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class StartGameButtonView : MonoBehaviour
    {
        [SerializeField]
        Button _button;

        void Awake() => _button.onClick.AddListener(() => GameStateSystem.RequestStateChange(GameState.Gameplay));
    }
}