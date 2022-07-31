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
            Container.BindInterfacesAndSelfTo<PresentationMainController>().AsSingle();
            Container.BindInterfacesAndSelfTo<AudioController>().AsSingle();
            Container.BindInterfacesAndSelfTo<VFXController>().AsSingle();
            Container.BindInterfacesAndSelfTo<PresentationViewModel>().AsSingle().NonLazy();
            Container.Bind<PresentationReferenceHolder>().AsSingle().NonLazy();
        }
    }
}