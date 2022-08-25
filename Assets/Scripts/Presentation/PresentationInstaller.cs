using Presentation.Controllers;
using Presentation.ViewModels;
using Shared.DependencyInjector;

namespace Presentation
{
    public class PresentationInstaller : AbstractInstaller
    {
        public override void InstallBindings()
        {
            // controllers
            Container.Bind<PresentationMainController>().AsSingle();
            Container.Bind<AudioController>().AsSingle();
            Container.Bind<VFXController>().AsSingle();
            Container.Bind<PresentationViewModel>().AsSingle();
            Container.Bind<PresentationReferenceHolder>().AsSingle();
        }
    }
}