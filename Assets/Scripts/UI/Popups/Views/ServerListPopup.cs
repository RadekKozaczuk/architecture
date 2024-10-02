#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Presentation.ViewModels;
using GameLogic.ViewModels;
using UnityEngine.UI;
using Core.Systems;
using UnityEngine;
using Core.Enums;
using UI.Config;
using Core.Dtos;
using UI.Views;
using TMPro;
using Core;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class ServerListPopup : AbstractPopup
    {
        [SerializeField]
        RectTransform _list;

        [SerializeField]
        Button _join;

        [SerializeField]
        Button _createServer;

        [SerializeField]
        Button _refresh;

        [SerializeField]
        Toggle _createServerToggle;

        [SerializeField]
        TMP_Text _ipv4InputField;

        [SerializeField]
        TMP_Text _portInputField;

        static readonly UIConfig _config;

        public ServerListPopup()
            : base(PopupType.ServerList) { }

        void Awake()
        {
            _createServer.onClick.AddListener(() =>
            {
                PresentationViewModel.PlaySound(Sound.ClickSelect);
                PopupSystem.ShowPopup(_createServerToggle.isOn ? PopupType.CreateServer : PopupType.CreateLobby);
            });

            _join.onClick.AddListener(() =>
            {
                CoreData.MachineRole = MachineRole.Client;

                CoreData.IsMultiplayer = true;
                CoreData.CurrentLevel = Level.HubLocation;

                GameLogicViewModel.SetConnectionData(_ipv4InputField.text, _portInputField.text);

                // this will start the netcode client
                GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CoreData.CurrentLevel});
            });

            _refresh.interactable = !GameLogicViewModel.WebRequestInProgress;
            _refresh.onClick.AddListener(RefreshAction);

            RefreshAction();
        }

        async void RefreshAction()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            _refresh.interactable = false;

            // cleanup
            foreach (Transform child in _list.transform)
                Destroy(child.gameObject);

            // add servers
            List<ServerDto> servers = await GameLogicViewModel.GetServersAsync();
            foreach (ServerDto server in servers)
            {
                if (server.status != ServerStatus.Allocated.ToString().ToUpper())
                    continue;

                ServerListElementView view = Instantiate(_config.ServerListElementView, _list.transform);

                // todo: no idea how to retrieve player count (current/max)
                string id = server.id.ToString();
                view.Initialize("Dedicated Server-" + id, server.ip, server.port);
            }

            // add lobbies
            LobbyDto[] lobbies = await GameLogicViewModel.GetLobbiesAsync();
            foreach (LobbyDto lobby in lobbies)
            {
                ServerListElementView view = Instantiate(_config.ServerListElementView, _list.transform);
                view.Initialize(lobby.LobbyName, lobby.LobbyId, 0000);
            }

            _refresh.interactable = true;
        }
    }
}