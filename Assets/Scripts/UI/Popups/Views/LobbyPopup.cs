using System.Collections.Generic;
using Common;
using Common.Enums;
using GameLogic.ViewModels;
using TMPro;
using UI.Config;
using UI.Views;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Views
{
	[DisallowMultipleComponent]
	class LobbyPopup : AbstractPopupView
	{
		[SerializeField]
		TextMeshProUGUI _lobbyName;

		[SerializeField]
		TextMeshProUGUI _lobbyCode;

		[SerializeField]
		Button _start;

		[SerializeField]
		Button _leave;

		[SerializeField]
		RectTransform _list;

		static readonly UIConfig _config;

		string _hostId;

		LobbyPopup() : base(PopupType.Lobby) { }

		void Awake()
		{
			_start.onClick.AddListener(StartAction);
			_start.interactable = false;
			_leave.onClick.AddListener(LeaveAction);
		}

		internal override void Initialize() { }

		internal override void Close()
		{
			// todo: leave the lobby and give away host to someone else
		}

		internal void UpdateLobby(string lobbyName, string lobbyCode, List<(string playerName, string playerId, bool isHost)> players)
		{
			string playerId = AuthenticationService.Instance.PlayerId;
			bool playerIsInLobby = false;

			foreach ((string playerName, string playerId, bool isHost) player in players)
				if (player.playerId == playerId)
					playerIsInLobby = true;

			if (playerIsInLobby)
				SetValues(lobbyName, lobbyCode, players);
			else
				ExitLobby();
		}

		internal void SetValues(string lobbyName, string lobbyCode, List<(string playerName, string playerId, bool isHost)> players)
		{
			_lobbyName.text = lobbyName;
			_lobbyCode.text = lobbyCode;

			foreach (Transform child in _list.transform)
				Destroy(child.gameObject);

			foreach ((string _, string playerId, bool isHost) in players)
				if (isHost)
					_hostId = playerId;

			bool isLocalPlayerHost = AuthenticationService.Instance.PlayerId == _hostId;

			_start.interactable = isLocalPlayerHost;

			foreach ((string playerName, string playerId, bool isHost) in players)
			{
				LobbyPlayerElementView view = Instantiate(_config.LobbyPlayerElementView, _list.transform);
				view.Initialize(playerName, playerId, isHost, isLocalPlayerHost);
			}
		}

		/// <summary>
		/// Set values for the host.
		/// </summary>
		internal void SetValues(string lobbyName, string lobbyCode, string playerName, string playerId)
		{
			_start.interactable = true;
			_hostId = playerId;
			_lobbyName.text = lobbyName;
			_lobbyCode.text = lobbyCode;
			LobbyPlayerElementView view = Instantiate(_config.LobbyPlayerElementView, _list.transform);
			view.Initialize(playerName, playerId, true, true);
		}

		void ExitLobby()
		{
			PopupSystem.CloseCurrentPopup();
			PopupSystem.ShowPopup(PopupType.LobbyList);
		}

		void StartAction()
		{
			_start.interactable = false;
			CommonData.CurrentLevel = Level.HubLocation;
			CommonData.IsMultiplayer = true;
			CommonData.PlayerId = PlayerId.Player1;
			GameLogicViewModel.StartGame();
		}

		void LeaveAction()
		{
			GameLogicViewModel.LeaveLobby();
			PopupSystem.CloseCurrentPopup();
		}
	}
}