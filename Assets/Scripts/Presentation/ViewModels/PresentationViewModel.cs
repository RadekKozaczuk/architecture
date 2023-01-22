using System.IO;
using Common.Enums;
using ControlFlow.DependencyInjector.Attributes;
using ControlFlow.DependencyInjector.Interfaces;
using JetBrains.Annotations;
using Presentation.Config;
using Presentation.Controllers;
using Shared.Systems;
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
            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(true);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(false);

            // spawn 5 VFXs around the player
            for (int i = 0; i < 5; i++)
            {
                float x = Random.Range(-5, 5);
                float z = Random.Range(-5, 5);
                int soundId = Random.Range(0, 4);

                PresentationReferenceHolder.VFXController.SpawnParticleEffect(VFX.HitEffect, new Vector3(x, 0f, z));
                AudioController.PlaySound((Sound)soundId, new Vector3(x, 0f, z));
            }
        }

        public static void GameplayOnExit() => PresentationReferenceHolder.AudioController.LoadMusic(Music.MainMenu);

        public static void LoadLevelData()
        {
            var levelSceneReferenceHolder = GameObject.FindWithTag("LevelSceneReferenceHolder").GetComponent<LevelSceneReferenceHolder>();
            PresentationData.Player = levelSceneReferenceHolder.Player;
        }

        public static void Movement(Vector2 movementInput) => PresentationData.Player.Move(movementInput.normalized);

        public static void SaveGame(BinaryWriter writer)
        {
            Transform playerTransform = PresentationData.Player.transform;

            SaveLoadSystem.Write(writer, playerTransform.position);
            SaveLoadSystem.Write(writer, playerTransform.rotation);
        }

        public static void SaveGame(BinaryReader reader)
        {
            Transform playerTransform = PresentationData.Player.transform;

            playerTransform.position = SaveLoadSystem.ReadVector3(reader);
            playerTransform.rotation = SaveLoadSystem.ReadQuaternion(reader);
        }
    }
}