using TMPro;
using UnityEngine;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class LobbyPlayerElementView : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _playerName;

        internal void Initialize(string playerName)
        {
            _playerName.text = playerName;
        }
    }
}