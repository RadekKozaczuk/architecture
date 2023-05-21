using System.Collections.Generic;
using Common.Enums;
using GameLogic.ViewModels;
using UI.Config;
using UI.Views;
using Unity.Services.Authentication;
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
        Button _create;

        [SerializeField]
        RectTransform _list;

        static readonly UIConfig _config;

        LobbyListPopup() : base(PopupType.LobbyList) { }

        void Awake()
        {
            _refresh.onClick.AddListener(RefreshAction);
            _join.onClick.AddListener(JoinAction);
            _join.interactable = GameLogicViewModel.SaveFileExist;
            _create.onClick.AddListener(CreateAction);
        }

        internal override void Initialize()
        {
            InitializeAsync();
        }

        internal override void Close()
        {
            // todo: do we need to deinititialize unity serices?

            AuthenticationService.Instance.SignOut();
        }

        async void InitializeAsync()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Sign In " + AuthenticationService.Instance.PlayerId);
                _refresh.interactable = true;
                _join.interactable = true;
                _create.interactable = true;
            };

            // this will create an account automatically without need to provide password or username
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        void RefreshAction()
        {
            GameLogicViewModel.RequestGetLobbies(LobbyQueryResultCallback);
        }

        void LobbyQueryResultCallback(List<(string lobbyName, int playerCount, int playerMax)> lobbies)
        {
            foreach ((string lobbyName, int playerCount, int playerMax) in lobbies)
            {
                LobbyListElementView view = Instantiate(_config.LobbyListElement, _list.transform);
                view.Initialize(lobbyName, playerCount, playerMax);
            }
        }

        static void JoinAction()
        {

        }

        static void CreateAction()
        {
            PopupSystem.ShowPopup(PopupType.CreateLobby);
        }
    }
}