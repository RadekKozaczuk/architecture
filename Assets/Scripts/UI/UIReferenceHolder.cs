#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable
using ControlFlow.DependencyInjector.Attributes;
using JetBrains.Annotations;
using UI.Controllers;
using UnityEngine.Scripting;

namespace UI
{
    /// <summary>
    /// Reference holders allow use to use injected classes in static classes.
    /// We cannot use <see cref="InjectAttribute" /> in static classes as static objects are not controlled via <see cref="Shared.Systems.Injector" />.
    /// Do not confuse reference holders with scene reference holders are these are two different things.
    /// </summary>
    [UsedImplicitly]
    class UIReferenceHolder
    {
        internal static UIMainController MainController => _instance._mainController;

        static UIReferenceHolder _instance;

        [Inject]
        readonly UIMainController _mainController;

        [Preserve]
        UIReferenceHolder() => _instance = this;
    }
}