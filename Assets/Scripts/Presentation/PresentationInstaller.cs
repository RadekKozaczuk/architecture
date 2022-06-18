using Presentation.Config;
using Presentation.Controllers;
using Presentation.ViewModels;
using UnityEngine;
using Zenject;

namespace Presentation
{
    public class PresentationInstaller : MonoInstaller
    {
        [SerializeField]
        AudioConfig _audioConfig;

        [SerializeField]
        VFXConfig _vfxConfig;

        public override void InstallBindings()
        {
            // configs
            Container.BindInstance(_audioConfig);
            Container.BindInstance(_vfxConfig);

            // controllers
            Container.BindInterfacesAndSelfTo<PresentationMainController>().AsSingle();
            Container.BindInterfacesAndSelfTo<AudioController>().AsSingle();
            Container.BindInterfacesAndSelfTo<VFXController>().AsSingle();
            Container.Bind<PresentationViewModel>().AsSingle().NonLazy();
            Container.Bind<PresentationReferenceHolder>().AsSingle().NonLazy();
        }
    }
}