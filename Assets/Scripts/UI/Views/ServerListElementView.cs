#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using GameLogic.ViewModels;
using UnityEngine.UI;
using Core.Systems;
using UnityEngine;
using Core.Enums;
using TMPro;
using Core;

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
            CoreData.MachineRole = MachineRole.Client;

            CoreData.IsMultiplayer = true;
            CoreData.CurrentLevel = Level.HubLocation;

            GameLogicViewModel.SetConnectionData(_ipText.text, _portText.text);

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