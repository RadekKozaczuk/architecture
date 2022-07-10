using Common.Enums;
using JetBrains.Annotations;
using Presentation.Controllers;
using Shared.DependencyInjector;
using Shared.DependencyInjector.Interfaces;

namespace Presentation.ViewModels
{
    [UsedImplicitly]
    public partial class PresentationViewModel : IInitializable
    {
        [Inject]
        readonly AudioController _audioController;

        static PresentationViewModel _instance;

        [Inject]
        PresentationMainController _presentationMainController;

        public void Initialize()
        {
            _instance = this;
        }

        public static void CustomUpdate()
        {
            _instance._presentationMainController.CustomUpdate();
        }

        public void PlayMusic(Music music)
        {
            _audioController.Play(music);
        }

        public static void FirstWolfGo()
        {
            PresentationSceneReferenceHolder.Wolf1.Initialize();
            //RotateNpcToPlayer(4535);
            //FollowPlayerTarget(34534, 10f);

            PresentationSceneReferenceHolder.Wolf1.DoWolfieThings();
        }

        public static void SecondWolfGo()
        {
            PresentationSceneReferenceHolder.Wolf2.Initialize();
            //RotateNpcToPlayer(4535);
            //FollowPlayerTarget(34534, 10f);

            PresentationSceneReferenceHolder.Wolf2.DoWolfieThings();
        }

        public static void OnCoreSceneLoaded() { }
    }
}