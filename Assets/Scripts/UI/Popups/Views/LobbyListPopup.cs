#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.Dtos;
using Common.Enums;
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
        internal RectTransform RectTransform;

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
        RectTransform _list;

        static readonly UIConfig _config;

        LobbyListPopup()
            : base(PopupType.LobbyList) { }

        void Awake()
        {
            PopupSystem.SetupPopupSize(RectTransform, false);

            _refresh.onClick.AddListener(RefreshAction);
            _join.onClick.AddListener(
                () =>
                {
                    PresentationViewModel.PlaySound(Sound.ClickSelect);
                    GameLogicViewModel.JoinLobbyById(LobbyListElementView.SelectedLobby!.LobbyId, JoinLobbyResultCallback);
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

        internal override void Initialize()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                RefreshAction();
                _create.interactable = true;
            }
        }

        internal override void Close() { }

        internal void SelectedLobbyChanged(bool canJoin) => _join.interactable = canJoin;

        void RefreshAction()
        {
            PresentationViewModel.PlaySound(Sound.ClickSelect);
            _refresh.interactable = false;
            const float Delay = 2f;
            StartCoroutine(EnableButtonAfterDelay(Delay));
            GameLogicViewModel.RequestGetLobbies(LobbyQueryResultCallback);
        }

        void LobbyQueryResultCallback(LobbyDto[] lobbies)
        {
            foreach (Transform child in _list.transform)
                Destroy(child.gameObject);

            for (int i = 0; i < lobbies.Length; i++)
            {
                LobbyDto lobby = lobbies[i];
                LobbyListElementView view = Instantiate(_config.LobbyListElement, _list.transform);
                view.Initialize(lobby.LobbyId, lobby.LobbyName, lobby.PlayerCount, lobby.PlayerMax);
            }
        }

        IEnumerator EnableButtonAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            _refresh.interactable = true;
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
                CommonData.PlayerId = PlayerId.Player2;
            }
        }

        void LobbyCodeInputOnValueChanged()
        {
            _lobbyCodeInput.text = _lobbyCodeInput.text.ToUpper();
            _joinByCode.interactable = _lobbyCodeInput.text.Length == 6;
        }
    }
}