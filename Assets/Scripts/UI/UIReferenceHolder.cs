using ControlFlow.DependencyInjector.Attributes;
using JetBrains.Annotations;
using UI.Controllers;

namespace UI
{
    /// <summary>
    /// Reference holders allow use to use injected classes in static classes.
    /// We cannot use <see cref="InjectAttribute"/> in static classes as static objects are not controlled via <see cref="Shared.Systems.Injector"/>.
    /// Do not confuse reference holders with scene reference holders are these are two different things.
    /// </summary>
    [UsedImplicitly]
    class UIReferenceHolder
    {
        internal static UIMainController MainController => _instance._mainController;

        static UIReferenceHolder _instance;

        [Inject]
        readonly UIMainController _mainController;

        UIReferenceHolder() => _instance = this;
    }
}