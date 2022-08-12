using JetBrains.Annotations;
using Presentation.ViewModels;
using Shared.DependencyInjector.Attributes;

namespace Presentation
{
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