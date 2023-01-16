using Common;
using JetBrains.Annotations;
using Presentation.ViewModels;
using UI.ViewModels;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Boot
{
    [UsedImplicitly]
    class BootController
    {
        [Preserve]
        BootController() => SceneManager.sceneLoaded += OnSceneLoaded;

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

            // any level scene
            if (scene.buildIndex >= 4)
                PresentationViewModel.LoadLevelData();
        }
    }
}