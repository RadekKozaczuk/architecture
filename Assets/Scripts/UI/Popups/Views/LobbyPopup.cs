using System.Collections.Generic;
using Common;
using Common.Enums;
using GameLogic.ViewModels;
using TMPro;
using UI.Config;
using UI.Views;
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
        Button _giveHost;

        [SerializeField]
        Button _start;

        [SerializeField]
        Button _leave;

        [SerializeField]
        RectTransform _list;

        static readonly UIConfig _config;

        LobbyPopup() : base(PopupType.Lobby) { }

        void Awake()
        {
            _giveHost.onClick.AddListener(GiveHostAction);
            _start.onClick.AddListener(StartAction);
            _start.interactable = UIData.HasCreatedLobby;
            _leave.onClick.AddListener(LeaveAction);
        }

        internal override void Initialize() { }

        internal override void Close()
        {
            // todo: leave the lobby and give away host to someone else
        }

        internal void SetValues(string lobbyName, List<(string playerName, string playerId, bool isHost)> players)
        {
            _lobbyName.text = lobbyName;

            foreach (Transform child in _list.transform)
                Destroy(child.gameObject);

            foreach ((string playerName, string playerId, bool isHost) in players)
            {
                LobbyPlayerElementView view = Instantiate(_config.LobbyPlayerElementView, _list.transform);
                view.Initialize(playerName, playerId, isHost);
            }
        }

        /// <summary>
        /// Set values for the host.
        /// </summary>
        internal void SetValues(string lobbyName, string playerName, string playerId)
        {
            _lobbyName.text = lobbyName;
            LobbyPlayerElementView view = Instantiate(_config.LobbyPlayerElementView, _list.transform);
            view.Initialize(playerName, playerId, true);
        }

        static void GiveHostAction() { }

        static void StartAction()
        {
            CommonData.CurrentLevel = Level.HubLocation;
            CommonData.IsMultiplayer = true;
            CommonData.IsServer = true;
            PopupSystem.CloseCurrentPopup();
            GameLogicViewModel.StartGame();
        }

        static void LeaveAction()
        {
            GameLogicViewModel.LeaveLobby();
            PopupSystem.CloseCurrentPopup();
        }
    }
}