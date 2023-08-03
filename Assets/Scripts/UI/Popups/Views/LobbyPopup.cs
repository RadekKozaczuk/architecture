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
			_start.interactable = UIData.HasCreatedLobby;
			_start.interactable = false;
			AuthenticationService.Instance.SignedIn += () => _start.interactable = true;
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

			foreach (var player in players){   
				if (player.playerId == playerId)
					playerIsInLobby = true;
			}

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

			foreach ((string playerName, string playerId, bool isHost) in players){
				if (isHost)
					_hostId = playerId;
			}


			foreach ((string playerName, string playerId, bool isHost) in players)
			{
				LobbyPlayerElementView view = Instantiate(_config.LobbyPlayerElementView, _list.transform);
				view.Initialize(playerName, playerId, isHost, AuthenticationService.Instance.PlayerId == _hostId);
			}
		}

		/// <summary>
		/// Set values for the host.
		/// </summary>
		internal void SetValues(string lobbyName, string lobbyCode, string playerName, string playerId)
		{
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

		static void StartAction()
		{
			CommonData.CurrentLevel = Level.HubLocation;
			CommonData.IsMultiplayer = true;
			PopupSystem.CloseCurrentPopup();
			CommonData.PlayerId = PlayerId.Player1;
			GameLogicViewModel.StartGame();
		}

		static void LeaveAction()
		{
			GameLogicViewModel.LeaveLobby();
			PopupSystem.CloseCurrentPopup();
		}
	}
}