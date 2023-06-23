using System.Threading.Tasks;
using Common;
using Common.Enums;
using Common.Systems;
using GameLogic.ViewModels;
using UI.Popups;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI.Views
{
    [DisallowMultipleComponent]
    class MainMenuView : MonoBehaviour
    {
        [SerializeField]
        Button _newGame;

        [SerializeField]
        Button _coop;

        [SerializeField]
        Button _loadGame;

        [SerializeField]
        Button _options;

        [SerializeField]
        Button _quit;

        [SerializeField]
        Button HostBtn;

        [SerializeField]
        Button ClientBtn;

        [SerializeField]
        InputField RelayInput;

        void Awake()
        {
            _newGame.onClick.AddListener(NewGame);
            _coop.onClick.AddListener(Coop);
            _loadGame.onClick.AddListener(LoadGame);
            _loadGame.interactable = GameLogicViewModel.SaveFileExist;
            _quit.onClick.AddListener(Quit);

            HostBtn.onClick.AddListener(HostAction);
            ClientBtn.onClick.AddListener(ClientAction);
        }

        static string RelayCode;

        async void HostAction()
        {
            await InitializeAsync();

            // Important: Once the allocation is created, you have ten seconds to BIND
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            RelayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Relay code: " + RelayCode);
            UISceneReferenceHolder.RelayCodeStatic.text = RelayCode;
            var relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            CommonData.IsMultiplayer = true;
            CommonData.CurrentLevel = Level.HubLocation;
            GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {((int)CommonData.CurrentLevel, true)});
        }

        async void ClientAction()
        {
            await InitializeAsync();

            RelayCode = RelayInput.text;
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(RelayCode);
            var serverData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
            NetworkManager.Singleton.StartClient();
            CommonData.IsMultiplayer = true;

            CommonData.CurrentLevel = Level.HubLocation;
            GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {((int)CommonData.CurrentLevel, true)});
        }

        async Task InitializeAsync()
        {
            Debug.Log("UnityServices.InitializeAsync & AuthenticationService.Instance.SignInAnonymouslyAsync call");

            await UnityServices.InitializeAsync();

            // tokens are stored in PlayerPrefs
            if (AuthenticationService.Instance.SessionTokenExists)
                Debug.Log($"Cached token exist. Recovering the existing credentials.");
            else
                Debug.Log($"Cached token not found. Creating new anonymous credentials.");

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Anonymous authentication completed successfully");
                Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
                Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
                //_refresh.interactable = true;
                //_create.interactable = true;

                Debug.Log($"IsSignedIn: {AuthenticationService.Instance.IsSignedIn}");
            };

            // this will create an account automatically without need to provide password or username
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        static void NewGame()
        {
            CommonData.CurrentLevel = Level.HubLocation;
            GameStateSystem.RequestStateChange(GameState.Gameplay, new[] {((int)CommonData.CurrentLevel, false)});
        }

        static void Coop()
        {
            PopupSystem.ShowPopup(PopupType.LobbyList);
        }

        static void LoadGame()
        {
            CommonData.LoadRequested = true;
            GameStateSystem.RequestStateChange(GameState.Gameplay);
        }

        static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}