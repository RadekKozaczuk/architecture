#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using Core.Enums;
using GameLogic.ViewModels;
using Presentation.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class CreateLobbyPopup : AbstractPopup
    {
        [SerializeField]
        TMP_InputField _input;

        [SerializeField]
        Slider _slider;

        [SerializeField]
        TextMeshProUGUI _playerCount;

        [SerializeField]
        Button _create;

        [SerializeField]
        Button _back;

        CreateLobbyPopup()
            : base(PopupType.CreateLobby) { }

        internal override void Initialize()
        {
            base.Initialize();

            _input.onValueChanged.AddListener(InputChanged);
            _slider.onValueChanged.AddListener(SliderChanged);
            _create.onClick.AddListener(CreateAction);
            _back.onClick.AddListener(() =>
            {
                PresentationViewModel.PlaySound(Sound.ClickSelect);
                PopupSystem.CloseCurrentPopup();
            });

            _input.text = "MyLobby";
            _slider.value = 2;
        }

        void SliderChanged(float value) => _playerCount.text = ((int)value).ToString();

        void InputChanged(string text) => _create.interactable = text.Length > 0;

        async void CreateAction()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            (bool success, string playerId, string lobbyCode) = await GameLogicViewModel.CreateLobby(_input.text, (int)_slider.value);

            if (success)
            {
                UIData.HasCreatedLobby = true;
                PopupSystem.CloseCurrentPopup();
                PopupSystem.CloseCurrentPopup();
                PopupSystem.ShowPopup(PopupType.Lobby);
                (PopupSystem.CurrentPopup as LobbyPopup)!.SetValues(_input.text, lobbyCode, CoreData.PlayerName, playerId);
            }
        }
    }
}