using Common;
using JetBrains.Annotations;
using Presentation.ViewModels;
using UI.ViewModels;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Boot
{
    [UsedImplicitly]
    class BootController
    {
        [Preserve]
        BootController() => SceneManager.sceneLoaded += OnSceneLoaded;

        static async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == Constants.CoreScene)
            {
                // this should not happen in the multiplayer context
                // when the machine act as a client
                if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsHost || !NetworkManager.Singleton.IsClient)
                    SceneManager.UnloadSceneAsync(Constants.BootScene);
                BootView.OnCoreSceneLoaded();
                PresentationViewModel.OnCoreSceneLoaded();
            }

            if (scene.buildIndex == Constants.UIScene)
                UIViewModel.OnUISceneLoaded();

            // level was loaded
            if (scene.buildIndex > 3)
                PresentationViewModel.OnLevelSceneLoaded();
        }
    }
}