#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections.Generic;
using Core.Enums;
using GameLogic.ViewModels;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine;

namespace UI.Popups.Views
{
    // todo: in the future signing in should already start in main menu (or even Boot)
    // todo: so that the player is not annoyed by this
    [DisallowMultipleComponent]
    class SigningInPopup : AbstractPopup // todo: In the future we can use this popup for signing in (login and password)
    {
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
            string? lobbyId = await GameLogicViewModel.GetJoinedLobbyAsync();

            PopupSystem.CloseCurrentPopup();

            // has user joined any lobbies
            if (lobbyId == null)
            {
                PopupSystem.ShowPopup(PopupType.ServerList);
            }
            else
            {
                PopupSystem.ShowPopup(PopupType.ReconnectToLobby);
                // todo: do we need this extra call just to retrieve lobby's name?
                (string id, string lobbyName) = await GameLogicViewModel.GetLobbyAsync(lobbyId);
                (PopupSystem.CurrentPopup as ReconnectToLobbyPopup)!.SetLobbyToReconnect(id, lobbyName);
            }
        }
    }
}