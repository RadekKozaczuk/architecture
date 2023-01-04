using ControlFlow.DependencyInjector.Attributes;
using ControlFlow.DependencyInjector.Interfaces;
using JetBrains.Annotations;
using UI.Controllers;
using UI.Systems;
using UnityEngine.Scripting;

namespace UI.ViewModels
{
    [UsedImplicitly]
    public class UIViewModel : IInitializable
    {
        static UIViewModel _instance;

        [Inject]
        readonly UIMainController _uiMainController;

        [Preserve]
        UIViewModel() { }

        public void Initialize() => _instance = this;

        public static void CustomFixedUpdate() => _instance._uiMainController.CustomFixedUpdate();

        public static void CustomUpdate() => _instance._uiMainController.CustomUpdate();

        public static void CustomLateUpdate() => _instance._uiMainController.CustomLateUpdate();

        public static void OnUISceneLoaded() => UIMainController.OnUISceneLoaded();

        public static void BootingOnEntry() { }

        public static void BootingOnExit() { }

        public static void MainMenuOnEntry() { }

        public static void MainMenuOnExit() { }

        public static void GameplayOnEntry() => InputSystem.IsActive = true;

        public static void GameplayOnExit() => InputSystem.IsActive = true;
    }
}