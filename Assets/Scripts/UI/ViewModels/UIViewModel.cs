using JetBrains.Annotations;
using Shared.DependencyInjector.Attributes;
using Shared.DependencyInjector.Interfaces;
using UI.Controllers;

namespace UI.ViewModels
{
    [UsedImplicitly]
    public class UIViewModel : IInitializable
    {
        static UIViewModel _instance;

        [Inject]
        UIMainController _uiMainController;
        
        public void Initialize() => _instance = this;

        public static void CustomUpdate() => _instance._uiMainController.CustomUpdate();
    }
}