using Common.Enums;
using ControlFlow.DependencyInjector.Attributes;
using ControlFlow.DependencyInjector.Interfaces;
using JetBrains.Annotations;
using Presentation.Config;
using Presentation.Controllers;
using UnityEngine.Scripting;

namespace Presentation.ViewModels
{
    [UsedImplicitly]
    public class PresentationViewModel : IInitializable
    {
        static PresentationViewModel _instance;

        static readonly AudioConfig _audioConfig;

        [Inject]
        readonly AudioController _audioController;

        [Inject]
        readonly PresentationMainController _presentationMainController;

        [Preserve]
        PresentationViewModel() { }

        public void Initialize()
        {
            _instance = this;
            _audioConfig.Initialize();
        }

        public static void CustomUpdate() => _instance._presentationMainController.CustomUpdate();

        public static void CustomFixedUpdate() => _instance._presentationMainController.CustomFixedUpdate();

        public static void CustomLateUpdate() => _instance._presentationMainController.CustomLateUpdate();

        public void PlayMusic(Music music) => _audioController.Play(music);

        public static void OnCoreSceneLoaded() => PresentationMainController.OnCoreSceneLoaded();

        public static void BootingOnExit() => _audioConfig.LoadMusic(Music.MainMenu);

        public static void MainMenuOnEntry()
        {
            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(false);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(true);
        }

        public static void MainMenuOnExit() => _audioConfig.UnloadMusic(Music.MainMenu);

        public static void GameplayOnEntry()
        {
            //SomeSystem.IsActive = true;
            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(true);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(false);
        }

        public static void GameplayOnExit()
        {
            //SomeSystem.IsActive = false;
            _audioConfig.LoadMusic(Music.MainMenu);
        }
    }
}