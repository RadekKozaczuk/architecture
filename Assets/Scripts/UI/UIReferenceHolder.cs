using JetBrains.Annotations;
using Shared.DependencyInjector.Attributes;
using UI.Controllers;

namespace UI
{
    /// <summary>
    /// Reference holders allow use to use injected classes in static classes.
    /// We cannot use <see cref="InjectAttribute"/> in static classes as static objects are not controlled via <see cref="Shared.DependencyInjector"/>.
    /// Do not confuse reference holders with scene reference holders are these are two different things.
    /// </summary>
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