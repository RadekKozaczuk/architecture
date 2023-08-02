using Common;
using Common.Enums;
using GameLogic.ViewModels;
using TMPro;
using UI.Config;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Views
{
	[DisallowMultipleComponent]
	class CreateLobbyPopup : AbstractPopupView
	{
		[SerializeField]
		TMP_InputField _input;

		[SerializeField]
		Slider _slider;

		[SerializeField]
		TextMeshProUGUI _playerCount;

		[SerializeField]
		Button _create;

		static readonly UIConfig _config;

		CreateLobbyPopup() : base(PopupType.CreateLobby) { }

		void Awake()
		{
			_input.onValueChanged.AddListener(InputChanged);
			_slider.onValueChanged.AddListener(SliderChanged);
			_create.onClick.AddListener(CreateAction);

			_input.text = "MyLobby";
			_slider.value = 2;
		}

		void SliderChanged(float value) => _playerCount.text = ((int)value).ToString();

		void InputChanged(string text) => _create.interactable = text.Length > 0;

		async void CreateAction()
		{
			(bool success, string playerId, string lobbyCode) = await GameLogicViewModel.CreateLobby(_input.text, (int)_slider.value);

			if (success)
			{
				UIData.HasCreatedLobby = true;
				PopupSystem.CloseCurrentPopup();
				PopupSystem.CloseCurrentPopup();
				PopupSystem.ShowPopup(PopupType.Lobby);
				(PopupSystem.CurrentPopup as LobbyPopup)!.SetValues(_input.text, lobbyCode, CommonData.PlayerName, playerId);
				CommonData.PlayerId = PlayerId.Player1;
			}
		}
    }
}