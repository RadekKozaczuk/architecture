#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Core.Enums;
using UnityEngine;
using UnityEngine.UI;
using Core;
using Core.Dtos;
using Core.Systems;
using GameLogic.ViewModels;
using Presentation.ViewModels;
using UI.Config;
using UI.Views;

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

        static readonly UIConfig _config;

        public ServerListPopup(PopupType type)
            : base(type) { }

        void Awake()
        {
            _join.onClick.AddListener(() =>
            {
                GameLogicViewModel.SetConnectionData();

                // todo: temporary disabled
                //KitchenGameMultiplayer.Instance.StartClient();

                // todo: RADEK's start client start here
                CoreData.IsMultiplayer = true;
                CoreData.CurrentLevel = Level.HubLocation;

                // this will start the netcode client
                GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CoreData.CurrentLevel});
            });

            _createServer.onClick.AddListener(() =>
            {
                PresentationViewModel.PlaySound(Sound.ClickSelect);
                PopupSystem.ShowPopup(PopupType.CreateLobby);
            });

            _refresh.interactable = !GameLogicViewModel.WebRequestInProgress;
            _refresh.onClick.AddListener(RefreshAction);

            // todo: destroy children
            // todo: instantiate new

            /*serverTemplate.gameObject.SetActive(false);
            foreach (Transform child in serverContainer)
            {
                if (child == serverTemplate)
                    continue;

                Destroy(child.gameObject);
            }*/
        }

        async void RefreshAction()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            _refresh.interactable = false;

            // cleanup
            foreach (Transform child in _list.transform)
                Destroy(child.gameObject);

            // add servers
            List<ServerDto> servers = await GameLogicViewModel.GetServers();
            foreach (ServerDto server in servers)
            {
                ServerListElementView view = Instantiate(_config.ServerListElementView, _list.transform);

                // todo: no idea how to retrieve player count (current/max)
                string id = server.Id.ToString();
                view.Initialize("Dedicated Server-" + id, server.Ip, server.Port);
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