#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Core.Enums;
using GameLogic.ViewModels;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;        // todo: should happen in GameLogic
using Unity.Services.Lobbies.Models; // todo: should happen in GameLogic
using Unity.Services.Vivox;
using UnityEngine;

namespace UI.Popups.Views
{
    // todo: in the future signing in should already start in main menu (or even Boot)
    // todo: so that the player is not annoyed by this
    [DisallowMultipleComponent]
    class SigningInPopup : AbstractPopup // todo: In the future we can use this popup for signing in (login and password)
    {
        List<string> _joinedLobbiesId = new();

        SigningInPopup()
            : base(PopupType.SigningIn) { }

        internal override void Initialize()
        {
            base.Initialize();

            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                HasUserJoinedLobbies();
                return;
            }

            InitializeAsync();
        }

        internal override void Close() { }

        async void InitializeAsync()
        {
            await UnityServices.InitializeAsync();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            AuthenticationService.Instance.ClearSessionToken();
#endif

            // this will create an account automatically without need to provide password or username
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            await VivoxService.Instance.InitializeAsync();

            // todo: should happen in GameLogic
            GameLogicViewModel.LoginVoiceChat(HasUserJoinedLobbies);
        }

        async void HasUserJoinedLobbies()
        {
            try
            {
                // todo: should happen in GameLogic
                _joinedLobbiesId = await LobbyService.Instance.GetJoinedLobbiesAsync();
            }
            catch (LobbyServiceException e) // todo: should happen in GameLogic
            {
                _joinedLobbiesId.Clear();
                Debug.Log(e);
            }

            PopupSystem.CloseCurrentPopup();

            // has user joined any lobbies
            if (_joinedLobbiesId.Count > 0)
            {
                string lobbyId = _joinedLobbiesId[^1];
                // todo: should happen in GameLogic
                Lobby lobby = await Lobbies.Instance.GetLobbyAsync(lobbyId);
                PopupSystem.ShowPopup(PopupType.ReconnectToLobby);
                (PopupSystem.CurrentPopup as ReconnectToLobbyPopup)!.SetLobbyToReconnect(lobby.Id, lobby.Name);
            }
            else
                PopupSystem.ShowPopup(PopupType.ServerList);
        }
    }
}