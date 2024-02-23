#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using GameLogic.ViewModels;
using TMPro;
using UI.Popups.Views;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class LobbyPlayerElementView : MonoBehaviour
    {
        [SerializeField]
        RectTransform _rectTransform;

        [SerializeField]
        TextMeshProUGUI _playerName;

        [SerializeField]
        Image _isHost;

        [SerializeField]
        Button _kick;

        [SerializeField]
        Button _giveHost;

        string _playerId;

        void Awake()
        {
            SetupSize();

            _kick.onClick.AddListener(() => GameLogicViewModel.KickPlayer(_playerId));
            _giveHost.onClick.AddListener(() => GameLogicViewModel.GiveHost(_playerId));
        }

        // todo: this should also take into account lobby ownership
        internal void Initialize(string playerName, string playerId, bool isHost, bool forHost)
        {
            _playerName.text = playerName;
            _playerId = playerId;
            Color c = _isHost.color;
            c.a = isHost ? 255 : 0f;
            _isHost.color = c;
            bool showHostMenu = forHost && !isHost;
            _kick.gameObject.SetActive(showHostMenu);
            _giveHost.gameObject.SetActive(showHostMenu);
        }

        void SetupSize()
        {
            LobbyPopup lobbyPopup = GetComponentInParent<LobbyPopup>();
            float widthToSetup = lobbyPopup.RectTransform.sizeDelta.x * 0.6f;
            float heightToSetup = widthToSetup * 0.12f;
            _rectTransform.sizeDelta = new Vector2(widthToSetup, heightToSetup);
        }
    }
}