using Common.Enums;
using ControlFlow.DependencyInjector.Attributes;
using ControlFlow.DependencyInjector.Interfaces;
using JetBrains.Annotations;
using Presentation.Config;
using Presentation.Controllers;
using UnityEngine;
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
        readonly VFXController _vfxController;

        [Inject]
        readonly PresentationMainController _presentationMainController;

        [Preserve]
        PresentationViewModel() { }

        public void Initialize() => _instance = this;

        public static void CustomUpdate() => _instance._presentationMainController.CustomUpdate();

        public static void CustomFixedUpdate() => _instance._presentationMainController.CustomFixedUpdate();

        public static void CustomLateUpdate() => _instance._presentationMainController.CustomLateUpdate();

        public static void OnCoreSceneLoaded() => PresentationMainController.OnCoreSceneLoaded();

        public static void BootingOnExit() => PresentationReferenceHolder.AudioController.LoadMusic(Music.MainMenu);

        public static void MainMenuOnEntry()
        {
            PresentationReferenceHolder.AudioController.PlayMusicWhenReady(Music.MainMenu);
            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(false);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(true);
        }

        public static void MainMenuOnExit() => PresentationReferenceHolder.AudioController.StopMusic();

        public static void GameplayOnEntry()
        {
            //SomeSystem.IsActive = true;
            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(true);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(false);

            // spawn 10 VFXs around the player
            for (int i = 0; i < 10; i++)
            {
                float x = Random.Range(-5, 5);
                float z = Random.Range(-5, 5);
                float delay = Random.Range(0, 4);

                 PresentationReferenceHolder.VFXController.SpawnParticleEffect(VFX.HitEffect, new Vector3(x, 0f, z));
            }
        }

        public static void GameplayOnExit()
        {
            //SomeSystem.IsActive = false;
            PresentationReferenceHolder.AudioController.LoadMusic(Music.MainMenu);
        }
    }
}