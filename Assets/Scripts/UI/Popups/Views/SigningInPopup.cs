using System.Collections.Generic;
using Common.Enums;
using GameLogic.ViewModels;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Vivox;
using UnityEngine;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class SigningInPopup : AbstractPopupView //In the future we can use this popup for signing in (login and password)
    {
        List<string> _joinedLobbiesId = new();

        SigningInPopup()
            : base(PopupType.SigningIn) { }

        internal override void Initialize()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                CheckIsUserHasJoinedLobbies();
                return;
            }

            InitializeAsync();
        }

        internal override void Close() { }

        async void InitializeAsync()
        {
            Debug.Log("UnityServices.InitializeAsync & AuthenticationService.Instance.SignInAnonymouslyAsync call");

            await UnityServices.InitializeAsync();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            AuthenticationService.Instance.ClearSessionToken();
#endif
            // tokens are stored in PlayerPrefs
            if (AuthenticationService.Instance.SessionTokenExists)
                Debug.Log("Cached token exist. Recovering the existing credentials.");
            else
                Debug.Log("Cached token not found. Creating new anonymous credentials.");

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Anonymous authentication completed successfully");
                Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
                Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
                Debug.Log($"IsSignedIn: {AuthenticationService.Instance.IsSignedIn}");
            };

            // this will create an account automatically without need to provide password or username
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            //VivoxService.Instance.Initialize();
            //GameLogicViewModel.LoginVoiceChat(CheckIsUserHasJoinedLobbies);
            CheckIsUserHasJoinedLobbies();
        }

        async void CheckIsUserHasJoinedLobbies()
        {
            try
            {
                _joinedLobbiesId = await LobbyService.Instance.GetJoinedLobbiesAsync();
            }
            catch (LobbyServiceException e)
            {
                _joinedLobbiesId.Clear();
                Debug.Log(e);
            }

            PopupSystem.CloseCurrentPopup();

            // has user joined any lobbies
            if (_joinedLobbiesId.Count > 0)
            {
                string lobbyId = _joinedLobbiesId[^1];
                Lobby lobby = await Lobbies.Instance.GetLobbyAsync(lobbyId);
                PopupSystem.ShowPopup(PopupType.ReconnectToLobby);
                (PopupSystem.CurrentPopup as ReconnectToLobbyPopup)!.SetLobbyToReconnect(lobby.Id, lobby.Name);
            }
            else
                PopupSystem.ShowPopup(PopupType.LobbyList);
        }
    }
}