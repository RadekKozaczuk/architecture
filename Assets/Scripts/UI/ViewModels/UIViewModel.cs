using ControlFlow.DependencyInjector.Attributes;
using ControlFlow.DependencyInjector.Interfaces;
using JetBrains.Annotations;
using UI.Config;
using UI.Controllers;
using UnityEngine.Scripting;
using InputSystem = UI.Systems.InputSystem;

namespace UI.ViewModels
{
    [UsedImplicitly]
    public class UIViewModel : IInitializable
    {
        static readonly UIConfig _uiConfig;
        static UIViewModel _instance;

        [Inject]
        readonly UIMainController _uiMainController;

        [Preserve]
        UIViewModel() { }

        public void Initialize()
        {
            _instance = this;
            InputSystem.Initialize();
        }

        public static void CustomFixedUpdate() => _instance._uiMainController.CustomFixedUpdate();

        public static void CustomUpdate() => _instance._uiMainController.CustomUpdate();

        public static void CustomLateUpdate() => _instance._uiMainController.CustomLateUpdate();

        public static void OnUISceneLoaded() => UIMainController.OnUISceneLoaded();

        public static void BootingOnExit() { }

        public static void MainMenuOnEntry() { }

        public static void MainMenuOnExit() { }

        public static void GameplayOnEntry() => _uiConfig.InputActionAsset.FindActionMap("Player").Enable();

        public static void GameplayOnExit() => _uiConfig.InputActionAsset.FindActionMap("Player").Disable();
    }
}