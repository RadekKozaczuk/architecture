#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Core;
using Core.Dtos;
using Core.Enums;
using GameLogic.ViewModels;
using Presentation.ViewModels;
using TMPro;
using UI.Config;
using UI.Views;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class LobbyListPopup : AbstractPopup
    {
        [SerializeField]
        Button _refresh;

        [SerializeField]
        Button _join;

        [SerializeField]
        TMP_InputField _lobbyCodeInput;

        [SerializeField]
        TextMeshProUGUI _lobbyCodeInputPlaceholder;

        [SerializeField]
        Button _joinByCode;

        [SerializeField]
        Button _create;

        [SerializeField]
        Button _joinDedicated;

        [SerializeField]
        RectTransform _list;

        static readonly UIConfig _config;

        LobbyListPopup()
            : base(PopupType.LobbyList) { }

        internal override void Initialize()
        {
            base.Initialize();

            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                RefreshAction();
                _create.interactable = true;
            }

            _refresh.interactable = !GameLogicViewModel.WebRequestInProgress;
            _refresh.onClick.AddListener(RefreshAction);
            _join.onClick.AddListener(() =>
            {
                PresentationViewModel.PlaySound(Sound.ClickSelect);
                //GameLogicViewModel.JoinLobbyById(LobbyListElementView.SelectedLobby!.LobbyId, JoinLobbyResultCallback);
            }); // join the selected
            _join.interactable = false;

            _create.onClick.AddListener(() =>
            {
                PresentationViewModel.PlaySound(Sound.ClickSelect);
                PopupSystem.ShowPopup(PopupType.CreateLobby);
            });

            _joinByCode.onClick.AddListener(() =>
            {
                PresentationViewModel.PlaySound(Sound.ClickSelect);
                GameLogicViewModel.JoinLobbyByCode(_lobbyCodeInput.text, JoinLobbyResultCallback);
            });
            _joinByCode.interactable = false;
            _lobbyCodeInput.onValueChanged.AddListener(_ => LobbyCodeInputOnValueChanged());
        }

        internal override void Close() { }

        internal void SelectedLobbyChanged(bool canJoin) => _join.interactable = canJoin;

        async void RefreshAction()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            _refresh.interactable = false;
            List<ServerDto> servers = await GameLogicViewModel.GetServersAsync();

            // cleanup
            foreach (Transform child in _list.transform)
                Destroy(child.gameObject);

            // todo: for now only take one
            //foreach (ServerDto server in servers)
            if (servers.Count > 0)
            {
                ServerDto? server = servers[0];
                LobbyListElementView view = Instantiate(_config.LobbyListElement, _list.transform);

                // todo: no idea how to retrieve player count
                // todo: dedicated servers have always max 16 players
                string id = server.id.ToString();
                view.Initialize(id, "Dedicated Server" + id, 0, 16);
            }

            // todo: in the future also lobbies
            //List<ServerDto> servers = await GameLogicViewModel.GetServers();

            Debug.Log(servers.Count);

            _refresh.interactable = false;
        }

        void LobbyQueryResultCallback(LobbyDto[] lobbies)
        {
            foreach (Transform child in _list.transform)
                Destroy(child.gameObject);

            foreach (LobbyDto lobby in lobbies)
            {
                LobbyListElementView view = Instantiate(_config.LobbyListElement, _list.transform);
                view.Initialize(lobby.LobbyId, lobby.LobbyName, lobby.PlayerCount, lobby.PlayerMax);
            }
        }

        void JoinLobbyResultCallback(string? lobbyName, string? lobbyCode, List<(string playerName, string playerId, bool isHost)> players)
        {
            if (lobbyName == null || lobbyCode == null)
            {
                _lobbyCodeInput.text = "";
                _lobbyCodeInputPlaceholder.text = "Wrong code";
                _lobbyCodeInputPlaceholder.color = Color.red;
            }
            else
            {
                PopupSystem.CloseCurrentPopup();
                PopupSystem.ShowPopup(PopupType.Lobby);
                (PopupSystem.CurrentPopup as LobbyPopup)!.SetValues(lobbyName, lobbyCode, players);
                CoreData.PlayerId = PlayerId.Player2;
            }
        }

        void LobbyCodeInputOnValueChanged()
        {
            _lobbyCodeInput.text = _lobbyCodeInput.text.ToUpper();
            _joinByCode.interactable = _lobbyCodeInput.text.Length == 6;
        }
    }
}