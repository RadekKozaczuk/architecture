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
            _refresh.onClick.AddListener(() => GameLogicViewModel.RequestGetLobbies(LobbyQueryResultCallback));
            _join.onClick.AddListener(() => GameLogicViewModel.JoinLobbyById(LobbyListElementView.SelectedLobby.LobbyId)); // join the selected
            _join.interactable = false;
            _create.onClick.AddListener(() => PopupSystem.ShowPopup(PopupType.CreateLobby));
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

        internal void SelectedLobbyChanged(bool selected)
        {
            _join.interactable = selected;
        }

        async void InitializeAsync()
        {
            Debug.Log("UnityServices.InitializeAsync & AuthenticationService.Instance.SignInAnonymouslyAsync call");

            await UnityServices.InitializeAsync();

            // tokens are stored in PlayerPrefs
            if (AuthenticationService.Instance.SessionTokenExists)
                Debug.Log($"Cached token exist. Recovering the existing credentials.");
            else
                Debug.Log($"Cached token not found. Creating new anonymous credentials.");

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Anonymous authentication completed successfully");
                Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
                Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
                _refresh.interactable = true;
                _create.interactable = true;

                Debug.Log($"IsSignedIn: {AuthenticationService.Instance.IsSignedIn}");
            };

            // this will create an account automatically without need to provide password or username
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        void LobbyQueryResultCallback(List<(string lobbyCode, string lobbyName, int playerCount, int playerMax)> lobbies)
        {
            foreach ((string lobbyCode, string lobbyName, int playerCount, int playerMax) in lobbies)
            {
                LobbyListElementView view = Instantiate(_config.LobbyListElement, _list.transform);
                view.Initialize(lobbyCode, lobbyName, playerCount, playerMax);
            }
        }
    }
}