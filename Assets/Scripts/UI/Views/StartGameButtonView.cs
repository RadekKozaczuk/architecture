#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Common.Enums;
using Common.Systems;
using Presentation.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class StartGameButtonView : MonoBehaviour
    {
        [SerializeField]
        Button _button;

        void Awake() => _button.onClick.AddListener(() =>
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            GameStateSystem.RequestStateChange(GameState.Gameplay);
        });
    }
}