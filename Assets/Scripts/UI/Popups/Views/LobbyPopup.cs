﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Core;
using Core.Enums;
using GameLogic.ViewModels;
using Presentation.ViewModels;
using TMPro;
using UI.Config;
using UI.Views;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class LobbyPopup : AbstractPopup
    {
        [SerializeField]
        TextMeshProUGUI _lobbyName;

        [SerializeField]
        TextMeshProUGUI _lobbyCode;

        [SerializeField]
        Button _mute;

        [SerializeField]
        Button _start;

        [SerializeField]
        Button _leave;

        [SerializeField]
        RectTransform _list;

        static readonly UIConfig _config;

        string _hostId;

        LobbyPopup()
            : base(PopupType.Lobby) { }

        internal override void Initialize()
        {
            base.Initialize();

            _start.onClick.AddListener(StartAction);
            _start.interactable = false;
            _leave.onClick.AddListener(LeaveAction);
            _mute.onClick.AddListener(MuteAction);
        }

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

        static void ExitLobby()
        {
            GameLogicViewModel.LeaveLobby();
            PopupSystem.CloseCurrentPopup();
            PopupSystem.ShowPopup(PopupType.LobbyList);
        }

        void StartAction()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            PopupSystem.CloseCurrentPopup();
            _start.interactable = false;
            CoreData.CurrentLevel = Level.HubLocation;
            CoreData.IsMultiplayer = true;
            GameLogicViewModel.StartGame();
        }

        static void LeaveAction()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            GameLogicViewModel.LeaveLobby();
            PopupSystem.CloseCurrentPopup();
        }

        void MuteAction()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            GameLogicViewModel.ToggleMuteInput();
            _mute.image.color = GameLogicViewModel.IsMuted ? Color.red : Color.white;
        }
    }
}