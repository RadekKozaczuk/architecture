using JetBrains.Annotations;
using Presentation.ViewModels;
using Shared.DependencyInjector.Attributes;

namespace Presentation
{
    /// <summary>
    /// Reference holders allow use to us injected classes in static classes.
    /// We cannot use <see cref="InjectAttribute"/> in static classes as static objects are not controlled via <see cref="Shared.DependencyInjector"/>.
    /// Do not confuse reference holders with scene reference holders are these are two different things.
    /// </summary>
    [UsedImplicitly]
    class PresentationReferenceHolder
    {
        public static PresentationViewModel ViewModel => _instance._viewModel;

        static PresentationReferenceHolder _instance;

        [Inject]
        PresentationViewModel _viewModel;

        PresentationReferenceHolder() => _instance = this;
    }
}