using System.Collections.Generic;
using Common;
using Common.Dtos;
using Common.Enums;
using GameLogic.ViewModels;
using UI.Config;
using UI.Views;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Views
{
	[DisallowMultipleComponent]
	class LobbyListPopup : AbstractPopupView
	{
		[SerializeField]
		Button _refresh;

		[SerializeField]
		Button _join;
		
		[SerializeField]
		Button _rejoin;
		
		[SerializeField]
		TMP_InputField _lobbyCodeInput;
		
		[SerializeField]
		Button _joinByCode;

		[SerializeField]
		Button _create;

		[SerializeField]
		RectTransform _list;

		static readonly UIConfig _config;
		
		List <string> _joinedLobbiesId = new();

		LobbyListPopup() : base(PopupType.LobbyList) { }

		void Awake()
		{
			_refresh.onClick.AddListener(() => GameLogicViewModel.RequestGetLobbies(LobbyQueryResultCallback));
			_join.onClick.AddListener(() => GameLogicViewModel.JoinLobbyById(LobbyListElementView.SelectedLobby.LobbyId, JoinLobbyResultCallback)); // join the selected
			_join.interactable = false;
			_create.onClick.AddListener(() => PopupSystem.ShowPopup(PopupType.CreateLobby));
			_joinByCode.onClick.AddListener(() => GameLogicViewModel.JoinLobbyByCode(_lobbyCodeInput.text, JoinLobbyResultCallback));
			_joinByCode.interactable = false;
			_lobbyCodeInput.onValueChanged.AddListener((string s) => _joinByCode.interactable = true);
			_rejoin.interactable = false;
		}
		
		async void GetJoindedLobbies()
		{
			_joinedLobbiesId = await LobbyService.Instance.GetJoinedLobbiesAsync();
			if (_joinedLobbiesId.Count > 0)
			{
				_rejoin.interactable = true;
				_rejoin.onClick.AddListener(() => GameLogicViewModel.RejoinToLobby(_joinedLobbiesId[_joinedLobbiesId.Count - 1], JoinLobbyResultCallback));
			}
		}

		internal override void Initialize()
		{
			InitializeAsync();
		}

		internal override void Close()
		{
			// todo: do we need to deinititialize unity services?

			AuthenticationService.Instance.SignOut();
		}

		internal void SelectedLobbyChanged(bool selected) => _join.interactable = selected;

		async void InitializeAsync()
		{
			if (UnityServices.State == ServicesInitializationState.Initialized)
			{
				//GameLogicViewModel.RequestGetLobbies(LobbyQueryResultCallback);
				GetJoindedLobbies();
				_refresh.interactable = true;
				_create.interactable = true;
				return;
			}
				
			Debug.Log("UnityServices.InitializeAsync & AuthenticationService.Instance.SignInAnonymouslyAsync call");

			await UnityServices.InitializeAsync();
			AuthenticationService.Instance.ClearSessionToken();
			// tokens are stored in PlayerPrefs
			if (AuthenticationService.Instance.SessionTokenExists)
				Debug.Log($"Cached token exist. Recovering the existing credentials.");
			else
				Debug.Log($"Cached token not found. Creating new anonymous credentials.");

			AuthenticationService.Instance.SignedIn += () =>
			{
				Debug.Log($"Anonymous authentication completed successfully");
				Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
				Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
				_refresh.interactable = true;
				_create.interactable = true;

				Debug.Log($"IsSignedIn: {AuthenticationService.Instance.IsSignedIn}");
				
			};

			// this will create an account automatically without need to provide password or username
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
			GameLogicViewModel.RequestGetLobbies(LobbyQueryResultCallback);
			GetJoindedLobbies();
		}

		void LobbyQueryResultCallback(LobbyDto[] lobbies)
		{
			foreach (Transform child in _list.transform)
				Destroy(child.gameObject);

			for (int i = 0; i < lobbies.Length; i++)
			{
				LobbyDto lobby = lobbies[i];
				LobbyListElementView view = Instantiate(_config.LobbyListElement, _list.transform);
				view.Initialize(lobby.LobbyId, lobby.LobbyName, lobby.PlayerCount, lobby.PlayerMax);
			}
		}

		static void JoinLobbyResultCallback(string lobbyName, string lobbyCode, List<(string playerName, string playerId, bool isHost)> players)
		{
			PopupSystem.CloseCurrentPopup();
			PopupSystem.ShowPopup(PopupType.Lobby);
			(PopupSystem.CurrentPopup as LobbyPopup)!.SetValues(lobbyName, lobbyCode, players);
			CommonData.PlayerId = PlayerId.Player2;
		}
	}
}