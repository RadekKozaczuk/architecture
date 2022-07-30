using Common.Enums;
using JetBrains.Annotations;
using Presentation.Controllers;
using Shared.DependencyInjector.Attributes;
using Shared.DependencyInjector.Interfaces;

namespace Presentation.ViewModels
{
    [UsedImplicitly]
    public partial class PresentationViewModel : IInitializable
    {
        static PresentationViewModel _instance;

        [Inject]
        readonly AudioController _audioController;

        [Inject]
        PresentationMainController _presentationMainController;

        public void Initialize() => _instance = this;

        public static void CustomUpdate() => _instance._presentationMainController.CustomUpdate();
        
        public static void CustomFixedUpdate() => _instance._presentationMainController.CustomFixedUpdate();

        public static void CustomLateUpdate() => _instance._presentationMainController.CustomLateUpdate();

        public void PlayMusic(Music music) => _audioController.Play(music);

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

        public static void OnCoreSceneLoaded() => PresentationMainController.OnCoreSceneLoaded();

        public static void GameplayOnEntry()
        {
            //SomeSystem.IsActive = true;
        }

        public static void GameplayOnExit()
        {
            //SomeSystem.IsActive = false;
        }
    }
}