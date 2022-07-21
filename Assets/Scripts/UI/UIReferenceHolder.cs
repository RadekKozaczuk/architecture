using JetBrains.Annotations;
using Shared.DependencyInjector.Attributes;
using UI.Controllers;

namespace UI
{
    [UsedImplicitly]
    class UIReferenceHolder
    {
        internal static UIMainController MainController => _instance._mainController;

        static UIReferenceHolder _instance;

        [Inject]
        UIMainController _mainController;

        UIReferenceHolder() => _instance = this;
    }
}