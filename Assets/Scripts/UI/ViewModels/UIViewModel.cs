using JetBrains.Annotations;
using Shared.DependencyInjector.Attributes;
using Shared.DependencyInjector.Interfaces;
using UI.Controllers;
using UI.Systems;

namespace UI.ViewModels
{
    [UsedImplicitly]
    public class UIViewModel : IInitializable
    {
        static UIViewModel _instance;

        [Inject]
        UIMainController _uiMainController;

        public void Initialize() => _instance = this;

        public static void CustomFixedUpdate() => _instance._uiMainController.CustomFixedUpdate();
        
        public static void CustomUpdate() => _instance._uiMainController.CustomUpdate();
        
        public static void CustomLateUpdate() => _instance._uiMainController.CustomLateUpdate();

        public static void OnUISceneLoaded() => UIMainController.OnUISceneLoaded();
        
        public static void GameplayOnEntry() => InputSystem.IsActive = true;
        
        public static void GameplayOnExit() => InputSystem.IsActive = true;
    }
}