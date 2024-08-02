#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
        TextMeshProUGUI ipText;
        [SerializeField]
        TextMeshProUGUI portText;

        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                string ipv4Address = ipText.text;
                ushort port = ushort.Parse(portText.text);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port);

                // todo: temporary disabled
                //KitchenGameMultiplayer.Instance.StartClient();
            });
        }

        public void SetServer(string ip, ushort port)
        {
            ipText.text = ip;
            portText.text = port.ToString();
        }
    }
}