using Common;
using Common.Enums;
using Common.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class NetworkButtonView : MonoBehaviour
    {
        [SerializeField]
        Button _clientButton;

        void Awake()
        {
            _clientButton.onClick.AddListener(() =>
            {
                CommonData.IsMultiplayer = true;
                CommonData.IsClient = true;
                CommonData.CurrentLevel = Level.HubLocation;
                GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {(int)CommonData.CurrentLevel});
            });
        }
    }
}