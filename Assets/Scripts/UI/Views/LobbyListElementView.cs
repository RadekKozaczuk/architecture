#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using TMPro;
using UI.Popups;
using UI.Popups.Views;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace UI.Views
{
    [DisallowMultipleComponent]
    class LobbyListElementView : MonoBehaviour
    {
        internal static LobbyListElementView? SelectedLobby
        {
            get => _selectedLobby;
            private set
            {
                _selectedLobby = value;
                var popup = (LobbyListPopup)PopupSystem.CurrentPopup!;
                popup.SelectedLobbyChanged(_selectedLobby != null && _canJoin); // if any is selected
            }
        }
        static LobbyListElementView? _selectedLobby;

        internal string LobbyId;

        [SerializeField]
        RectTransform _rect;

        [SerializeField]
        TextMeshProUGUI _name;

        [SerializeField]
        TextMeshProUGUI _playerCount;

        [SerializeField]
        Button _button;

        [SerializeField]
        Image _image;

        static bool _canJoin;

        void Awake()
        {
            _button.onClick.AddListener(ButtonAction);
            PopupSystem.SetupPopupElementSize(transform.parent.GetComponent<RectTransform>(), _rect);
        }

        internal void Initialize(string lobbyId, string lobbyName, int playerCount, int playerMax)
        {
            LobbyId = lobbyId;
            _name.text = lobbyName;
            _playerCount.text = $"{playerCount}/{playerMax}";
            _canJoin = playerCount < playerMax;
        }

        void ButtonAction()
        {
            // first click
            if (SelectedLobby == null)
                SetAlpha(this, 100);
            else if (SelectedLobby == this)
                SetAlpha(null, 0);
            else
                SetAlpha(this, 100);

            void SetAlpha(LobbyListElementView? selectedLobby, float alpha)
            {
                SelectedLobby = selectedLobby;
                Color color = _image.color;
                color.a = alpha;
                _image.color = color;
            }
        }
    }
}