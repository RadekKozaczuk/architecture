#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using Core.Enums;
using Core.Systems;
using GameLogic.ViewModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    class ServerListElementView : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _nameText;

        [SerializeField]
        TextMeshProUGUI _ipText;

        [SerializeField]
        TextMeshProUGUI _portText;

        void Awake() => GetComponent<Button>().onClick.AddListener(() =>
        {
            GameLogicViewModel.SetConnectionData(_ipText.text, _portText.text);

            CoreData.IsMultiplayer = true;
            CoreData.CurrentLevel = Level.HubLocation;

            // this will start the netcode client
            GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CoreData.CurrentLevel});
        });

        internal void Initialize(string serverName, string ip, int port)
        {
            _nameText.text = serverName;
            _ipText.text = ip;
            _portText.text = port.ToString();
        }
    }
}