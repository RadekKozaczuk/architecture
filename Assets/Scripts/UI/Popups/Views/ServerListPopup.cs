#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core.Enums;
using UnityEngine;
using UnityEngine.UI;
using Core;
using Core.Systems;
using GameLogic.ViewModels;
using UI.Config;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    class ServerListPopup : AbstractPopup
    {
        [SerializeField]
        Transform _serverContainer;

        [SerializeField]
        Button _joinButton;

        [SerializeField]
        Button _createServerButton;

        static readonly UIConfig _config;

        public ServerListPopup(PopupType type)
            : base(type) { }

        void Awake()
        {
            _joinButton.onClick.AddListener(() =>
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

            _createServerButton.onClick.AddListener(() =>
            {

            });

            // todo: destroy children
            // todo: intantiate new

            /*serverTemplate.gameObject.SetActive(false);
            foreach (Transform child in serverContainer)
            {
                if (child == serverTemplate)
                    continue;

                Destroy(child.gameObject);
            }*/
        }
    }
}