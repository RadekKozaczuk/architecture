using Common;
using JetBrains.Annotations;
using Presentation.ViewModels;
using UI.ViewModels;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Boot
{
    [UsedImplicitly]
    class FlowController
    {
        [Preserve]
        FlowController() => SceneManager.sceneLoaded += OnSceneLoaded;

        static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == Constants.CoreScene)
            {
                SceneManager.UnloadSceneAsync(Constants.BootScene);
                BootView.OnCoreSceneLoaded();
                PresentationViewModel.OnCoreSceneLoaded();
            }

            if (scene.buildIndex == Constants.UIScene)
                UIViewModel.OnUISceneLoaded();
        }
    }
}