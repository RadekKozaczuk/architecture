using System.Collections.Generic;
using Common;
using Common.Enums;
using Common.Systems;
using GameLogic.ViewModels;
using UI.Config;
using UI.Systems;
using UI.Views;
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
            //_hub.interactable = CommonData.CurrentLevel != Level.HubLocation;
        }

        async void RefreshAction()
        {
            List<(string, int, int)> qq = await LobbySystem.ListLobbies();

            foreach ((string lobbyName, int playerCount, int playerMax) in qq)
            {
                LobbyListElementView view = Instantiate(_config.LobbyListElement, _list.transform);
                view.Initialize(lobbyName, playerCount, playerMax);
            }

            //GameLogicViewModel.SaveGame();
            //_loadGame.interactable = GameLogicViewModel.SaveFileExist;
        }

        static void JoinAction()
        {

        }

        static void CreateAction()
        {
            // open create lobby popup
        }
    }
}