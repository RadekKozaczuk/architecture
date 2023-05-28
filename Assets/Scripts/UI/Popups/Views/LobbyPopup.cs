using System.Collections.Generic;
using Common.Enums;
using GameLogic.ViewModels;
using TMPro;
using UI.Config;
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
        Button _kick;

        [SerializeField]
        Button _leave;

        [SerializeField]
        RectTransform _list;

        static readonly UIConfig _config;

        LobbyPopup() : base(PopupType.Lobby) { }

        void Awake()
        {
            _giveHost.onClick.AddListener(GiveHostAction);
            _kick.onClick.AddListener(KickAction);
            _leave.onClick.AddListener(LeaveAction);
        }

        internal override void Initialize()
        {
        }

        internal override void Close()
        {
            // todo: leave the lobby and give away host to someone else
        }

        internal void SetValues(string lobbyName, List<(string playerName, bool isHost)> players)
        {
            _lobbyName.text = lobbyName;
        }

        void GiveHostAction() { }

        static void KickAction()
        {
            GameLogicViewModel.KickPlayer("f");
        }

        static void LeaveAction()
        {
            GameLogicViewModel.LeaveLobby();
        }
    }
}