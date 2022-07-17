using Common;
using JetBrains.Annotations;
using Presentation.Controllers;
using Presentation.ViewModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Boot.Controllers
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

                // have to be here because Boot assembly does not see UI
                // and the view must be in UI not in common cause it touches GameLogic stuff
#if UNITY_EDITOR
                GameObject debug = GameObject.Find("DebugCommands");
                //debug.AddComponent<DebugCommandsView>();
#endif

                MainBootController.OnCoreSceneLoaded();
                PresentationMainController.OnCoreSceneLoaded();
                PresentationViewModel.OnCoreSceneLoaded();
            }
        }
    }
}