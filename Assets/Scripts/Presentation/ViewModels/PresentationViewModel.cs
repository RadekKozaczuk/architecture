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

        public static void Troll()
        {
            PresentationSceneReferenceHolder.Enemy.Initialize();
            //RotateNpcToPlayer(4535);
            //FollowPlayerTarget(34534, 10f);

            GoAndFaceTargetWithOffset(123, 123, 10f);
        }
        
        public static void OnCoreSceneLoaded()
        {
            
        }
    }
}