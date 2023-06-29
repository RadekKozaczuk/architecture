using GameLogic.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class LobbyPlayerElementView : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _playerName;

        [SerializeField]
        Image _isHost;

        [SerializeField]
        Button _kick;

        string _playerId;

        void Awake() => _kick.onClick.AddListener(() => GameLogicViewModel.KickPlayer(_playerId));

        // todo: this should also take into account lobby ownership
        internal void Initialize(string playerName, string playerId, bool isHost)
        {
            _playerName.text = playerName;
            _playerId = playerId;
            Color c = _isHost.color;
            c.a = isHost ? 255 : 0f;
            _isHost.color = c;
            _kick.gameObject.SetActive(!isHost);
        }
    }
}