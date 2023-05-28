using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class LobbyListElementView : MonoBehaviour
    {
        internal static LobbyListElementView SelectedLobby;
        internal string LobbyCode;

        [SerializeField]
        TextMeshProUGUI _name;

        [SerializeField]
        TextMeshProUGUI _playerCount;

        [SerializeField]
        Button _button;

        void Awake()
        {
            _button.onClick.AddListener(() => SelectedLobby = this);
        }

        internal void Initialize(string lobbyCode, string lobbyName, int playerCount, int playerMax)
        {
            LobbyCode = lobbyCode;
            _name.text = lobbyName;
            _playerCount.text = $"{playerCount}/{playerMax}";
        }
    }
}