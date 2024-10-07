#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Core;
using Core.Enums;
using GameLogic.ViewModels;
using TMPro;
using UI.Popups;
using UI.Popups.Views;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace UI.Views
{
    [DisallowMultipleComponent]
    class LobbyListElementView : MonoBehaviour
    {
        internal string LobbyId;

        [SerializeField]
        RectTransform _rect;

        [SerializeField]
        TextMeshProUGUI _name;

        [SerializeField]
        TextMeshProUGUI _playerCount;

        [SerializeField]
        Button _button;

        [SerializeField]
        Image _image;
        
        void Awake()
        {
            _button.onClick.AddListener(() => GameLogicViewModel.JoinLobbyById(LobbyId, JoinLobbyResultCallback));
            PopupSystem.SetupPopupElementSize(transform.parent.GetComponent<RectTransform>(), _rect);
        }

        internal void Initialize(string lobbyId, string lobbyName, int playerCount, int playerMax)
        {
            LobbyId = lobbyId;
            _name.text = lobbyName;
            _playerCount.text = $"{playerCount}/{playerMax}";
        }

        void JoinLobbyResultCallback(string? lobbyName, string? lobbyCode, List<(string playerName, string playerId, bool isHost)> players)
        {
            if (lobbyName == null || lobbyCode == null)
                return;

            PopupSystem.CloseCurrentPopup();
            PopupSystem.ShowPopup(PopupType.Lobby);
            (PopupSystem.CurrentPopup as LobbyPopup)!.SetValues(lobbyName, lobbyCode, players);
            CoreData.PlayerId = PlayerId.Player2;
        }
    }
}