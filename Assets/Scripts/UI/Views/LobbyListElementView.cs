using TMPro;
using UI.Popups;
using UI.Popups.Views;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class LobbyListElementView : MonoBehaviour
    {
        internal static LobbyListElementView SelectedLobby
        {
            get => _selectedLobby;
            private set
            {
                _selectedLobby = value;
                var popup = (LobbyListPopup)PopupSystem.CurrentPopup;
                popup!.SelectedLobbyChanged(_selectedLobby != null); // if any is selected
            }
        }
        static LobbyListElementView _selectedLobby;

        internal string LobbyId;

        [SerializeField]
        TextMeshProUGUI _name;

        [SerializeField]
        TextMeshProUGUI _playerCount;

        [SerializeField]
        Button _button;

        void Awake() => _button.onClick.AddListener(ButtonAction);

        internal void Initialize(string lobbyId, string lobbyName, int playerCount, int playerMax)
        {
            LobbyId = lobbyId;
            _name.text = lobbyName;
            _playerCount.text = $"{playerCount}/{playerMax}";
        }

        void ButtonAction()
        {
            // first click
            if (SelectedLobby == null)
                SelectedLobby = this;
            else
                SelectedLobby = SelectedLobby == this ? null : this;
        }
    }
}