using ControlFlow.DependencyInjector.Attributes;
using JetBrains.Annotations;
using Presentation.Controllers;
using Presentation.ViewModels;

namespace Presentation
{
    /// <summary>
    /// Reference holders allow use to us injected classes in static classes.
    /// We cannot use <see cref="InjectAttribute" /> in static classes as static objects are not controlled via <see cref="Shared.Systems.Injector" />.
    /// Do not confuse reference holders with scene reference holders are these are two different things.
    /// </summary>
    [UsedImplicitly]
    class PresentationReferenceHolder
    {
        internal static PresentationViewModel ViewModel => _instance._viewModel;

        internal static AudioController AudioController => _instance._audioController;

        static PresentationReferenceHolder _instance;

        [Inject]
        readonly PresentationViewModel _viewModel;

        [Inject]
        readonly AudioController _audioController;

        PresentationReferenceHolder() => _instance = this;
    }
}