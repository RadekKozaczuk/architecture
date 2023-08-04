﻿using Common;
using Common.Dtos;
using Common.Enums;
using GameLogic.ViewModels;
using UI.Config;
using UI.Views;
using TMPro;
using System.Collections;
using System.Collections.Generic;
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
		TMP_InputField _lobbyCodeInput;

		[SerializeField]
		Button _joinByCode;

		[SerializeField]
		Button _create;

		[SerializeField]
		RectTransform _list;

		static readonly UIConfig _config;
		LobbyListPopup() : base(PopupType.LobbyList) { }

		void Awake()
		{
			_refresh.onClick.AddListener(RefreshAction);
			_join.onClick.AddListener(() => GameLogicViewModel.JoinLobbyById(LobbyListElementView.SelectedLobby.LobbyId, JoinLobbyResultCallback)); // join the selected
			_join.interactable = false;
			_create.onClick.AddListener(() => PopupSystem.ShowPopup(PopupType.CreateLobby));
			_joinByCode.onClick.AddListener(() => GameLogicViewModel.JoinLobbyByCode(_lobbyCodeInput.text, JoinLobbyResultCallback));
			_joinByCode.interactable = false;
			_lobbyCodeInput.onValueChanged.AddListener(_ => LobbyCodeInputOnValueChanged());
		}

		internal override void Initialize()
		{
			if (UnityServices.State == ServicesInitializationState.Initialized)
			{
				RefreshAction();
				_create.interactable = true;
				return;
			}
		}

		internal override void Close()	{	}

		internal void SelectedLobbyChanged(bool selected) => _join.interactable = selected;

		void RefreshAction()
		{
			_refresh.interactable = false;
			float delay = 2f;
			StartCoroutine(EnableButtonAfterDelay(delay));
			GameLogicViewModel.RequestGetLobbies(LobbyQueryResultCallback);
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

		private IEnumerator EnableButtonAfterDelay(float delay)
		{
			yield return new WaitForSeconds(delay);
			_refresh.interactable = true;
		}


		static void JoinLobbyResultCallback(string lobbyName, string lobbyCode, List<(string playerName, string playerId, bool isHost)> players)
		{
			PopupSystem.CloseCurrentPopup();
			PopupSystem.ShowPopup(PopupType.Lobby);
			(PopupSystem.CurrentPopup as LobbyPopup)!.SetValues(lobbyName, lobbyCode, players);
			CommonData.PlayerId = PlayerId.Player2;
		}

		void LobbyCodeInputOnValueChanged()
		{
			_lobbyCodeInput.text = _lobbyCodeInput.text.ToUpper();
			_joinByCode.interactable = _lobbyCodeInput.text.Length == 6;
		}
	}
}