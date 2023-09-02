using System.Collections.Generic;
using Common;
using Common.Enums;
using GameLogic.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class ReconnectToLobbyPopup : AbstractPopupView
    {
        [SerializeField]
        TextMeshProUGUI _lobbyToReconnect;

        [SerializeField]
        Button _join;

        [SerializeField]
        Button _lobbyList;

        string _lobbyToReconnectId;

        ReconnectToLobbyPopup()
            : base(PopupType.ReconnectToLobby) { }

        void Awake()
        {
            _lobbyList.interactable = true;
            _lobbyList.onClick.AddListener(BackToLobbyListAction);
        }

        internal override void Initialize() { }

        internal override void Close() { }

        internal void SetLobbyToReconnect(string lobbyId, string lobbyName)
        {
            _lobbyToReconnect.text = lobbyName;
            _lobbyToReconnectId = lobbyId;
            _join.onClick.AddListener(() => GameLogicViewModel.RejoinToLobby(lobbyId, JoinLobbyResultCallback));
            _join.interactable = true;
        }

        static void JoinLobbyResultCallback(string lobbyName, string lobbyCode, List<(string playerName, string playerId, bool isHost)> players)
        {
            PopupSystem.CloseCurrentPopup();
            PopupSystem.ShowPopup(PopupType.Lobby);
            (PopupSystem.CurrentPopup as LobbyPopup)!.SetValues(lobbyName, lobbyCode, players);
            CommonData.PlayerId = PlayerId.Player2;
        }

        void BackToLobbyListAction()
        {
            PopupSystem.CloseCurrentPopup();
            GameLogicViewModel.RemovePlayerFromLobby(_lobbyToReconnectId);
            PopupSystem.ShowPopup(PopupType.LobbyList);
        }
    }
}