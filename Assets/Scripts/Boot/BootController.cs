#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Common;
using JetBrains.Annotations;
using Presentation.ViewModels;
using UI.ViewModels;
using UnityEngine;
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

            // level was loaded
            if (scene.buildIndex > 3)
            {
                PresentationViewModel.OnLevelSceneLoaded();
                CheckObjectsName();
            }
        }

        static void CheckObjectsName()
        {
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject go in allObjects)
            {
                string Name = go.name;
                if (!char.IsUpper(Name, 0) && Name[0] is not '_') Debug.LogWarning("Object name " + go.name + " doesnt start with capital letter");
            }
        }
    }
}