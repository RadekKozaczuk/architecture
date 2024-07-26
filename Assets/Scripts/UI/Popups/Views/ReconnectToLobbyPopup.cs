#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
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
    class ReconnectToLobbyPopup : AbstractPopup
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

        internal override void Initialize()
        {
            base.Initialize();

            _lobbyList.interactable = true;
            _lobbyList.onClick.AddListener(BackToLobbyListAction);
        }

        internal override void Close() { }

        internal void SetLobbyToReconnect(string lobbyId, string lobbyName)
        {
            _lobbyToReconnect.text = lobbyName;
            _lobbyToReconnectId = lobbyId;
            _join.onClick.AddListener(() =>
            {
                PresentationViewModel.PlaySound(Sound.ClickSelect);
                GameLogicViewModel.RejoinToLobby(lobbyId, JoinLobbyResultCallback);
            });
            _join.interactable = true;
        }

        static void JoinLobbyResultCallback(string lobbyName, string lobbyCode, List<(string playerName, string playerId, bool isHost)> players)
        {
            PopupSystem.CloseCurrentPopup();
            PopupSystem.ShowPopup(PopupType.Lobby);
            (PopupSystem.CurrentPopup as LobbyPopup)!.SetValues(lobbyName, lobbyCode, players);
            CoreData.PlayerId = PlayerId.Player2;
        }

        void BackToLobbyListAction()
        {
            PopupSystem.CloseCurrentPopup();
            GameLogicViewModel.RemovePlayerFromLobby(_lobbyToReconnectId);
            PopupSystem.ShowPopup(PopupType.LobbyList);
        }
    }
}