#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Core;
using Core.Enums;
using Core.Systems;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class ServerListElementView : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _ipText;
        [SerializeField]
        TextMeshProUGUI _portText;

        void Awake() => GetComponent<Button>().onClick.AddListener(() =>
        {
            string ipv4Address = _ipText.text;
            ushort port = ushort.Parse(_portText.text);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port);

            // todo: temporary disabled
            //KitchenGameMultiplayer.Instance.StartClient();

            // todo: RADEK's start client start here
            CoreData.IsMultiplayer = true;
            CoreData.CurrentLevel = Level.HubLocation;

            // this will start the netcode client
            GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CoreData.CurrentLevel});
        });

        public void SetServer(string ip, ushort port)
        {
            _ipText.text = ip;
            _portText.text = port.ToString();
        }
    }
}