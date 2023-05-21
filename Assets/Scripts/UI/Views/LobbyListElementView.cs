using TMPro;
using UnityEngine;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class LobbyListElementView : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _name;

        [SerializeField]
        TextMeshProUGUI _playerCount;

        internal void Initialize(string lobbyName, int playerCount, int playerMax)
        {
            
        }
    }
}