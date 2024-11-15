#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.IO;
using Core;
using Core.Enums;
using Core.Systems;
using ControlFlow.DependencyInjector;
using JetBrains.Annotations;
using Presentation.Config;
using Presentation.Controllers;
using Presentation.Systems;
using Presentation.Views;
using Shared.Systems;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Presentation.ViewModels
{
    [UsedImplicitly]
    public class PresentationViewModel : IInitializable
    {
        static readonly PlayerConfig _playerConfig;

        [Inject]
        static readonly VFXController _vfxController;

        [Inject]
        static readonly PresentationMainController _presentationMainController;

        static LevelSceneReferenceHolder _level;

        [Preserve]
        PresentationViewModel() { }

        public void Initialize() { }
        public static void CustomUpdate() => _presentationMainController.CustomUpdate();

        public static void CustomFixedUpdate() => _presentationMainController.CustomFixedUpdate();

        public static void CustomLateUpdate() => _presentationMainController.CustomLateUpdate();

        public static void OnClientConnected(ulong clientId, int clientIndex)
        {
            if (clientId == 0)
                return;

            Transform spawnPoint = _level.GetSpawnPoint(clientIndex).transform;
            PlayerNetworkView networkPlayer = Object.Instantiate(_playerConfig.PlayerClientPrefab, spawnPoint.position, spawnPoint.rotation,
                PresentationSceneReferenceHolder.PlayerContainer);

            // this will be assigned only on the host
            PresentationData.NetworkPlayers.Add(clientId, networkPlayer);

            // spawn over the network
            networkPlayer.NetworkObj.SpawnWithOwnership(clientId, true);
            networkPlayer.gameObject.SetActive(true);
        }

        public static void OnClientDisconnected(ulong clientId, int clientIndex) =>
            PresentationData.NetworkPlayers.Remove(clientId);

        public static void OnCoreSceneLoaded() => PresentationMainController.OnCoreSceneLoaded();

        public static void BootingOnExit() { }

        public static void MainMenuOnEntry()
        {
            MusicSystem.LoadAndPlayWhenReady(Music.MainMenu);
            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(false);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(true);
        }

        public static void MainMenuOnExit()
        {
            MusicSystem.Stop();
            MusicSystem.Unload(Music.MainMenu);
        }

        public static void GameplayOnEntry()
        {
            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(true);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(false);

            Debug.Log($"GameplayOnEntry, frame: {Time.frameCount}");

            // load level data
            _level = GameObject.FindWithTag("LevelSceneReferenceHolder").GetComponent<LevelSceneReferenceHolder>();

            if (CoreData.IsMultiplayer)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    // Host is always 0
                    const ulong ID = 0;
                    Transform spawnPoint = _level.GetSpawnPoint((int)ID).transform;

                    if (PresentationData.NetworkPlayers.TryGetValue(ID, out PlayerNetworkView networkPlayer))
                    {
                        Transform transform = networkPlayer.transform;
                        transform.position = spawnPoint.position;
                        transform.rotation = spawnPoint.rotation;
                    }
                    else
                    {
                        // instantiate locally
                        // in network context objects can only be spawned in root - we cannot spawn under other objects.
                        // todo: player must be explicitly spawned in PlayerContainer because at the moment of state transition the main scene
                        // todo: is the MainMenuScene. This problem eventually fix itself as after MainMenuScene is unloaded CoreScene
                        // todo: becomes the new main scene.
                        // todo: however maybe GameStateMachine should control which scene is the main scene to avoid these problems in the future.
                        PlayerNetworkView player = Object.Instantiate(_playerConfig.PlayerServerPrefab, spawnPoint.position, spawnPoint.rotation,
                                                                      PresentationSceneReferenceHolder.PlayerContainer);

                        // this will be assigned only on the host
                        PresentationData.NetworkPlayers.Add(ID, player);

                        // spawn over the network
                        // Spawning in Netcode means to instantiate and/or spawn the object that is synchronized between all clients by the server.
                        // Only server can spawn multiplayer objects.
                        player.NetworkObj.Spawn(true);
                        player.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                SpawnSinglePlayer(_level.GetSpawnPoint(0));
            }

            bool loadGameRequested = (bool)GameStateSystem.GetTransitionParameter(StateTransitionParameter.LoadGameRequested)!;
            if (loadGameRequested)
            {
                Vector3 position = SaveLoadUtils.ReadVector3(CoreData.SaveGameReader!);
                Quaternion rotation = SaveLoadUtils.ReadQuaternion(CoreData.SaveGameReader!);
                Transform transform = PresentationData.Player.transform;
                transform.position = position;
                transform.rotation = rotation;
            }

            // spawn 5 VFXs around the player
            /*for (int i = 0; i < 5; i++)
            {
                float x = Random.Range(-5, 5);
                float z = Random.Range(-5, 5);
                int soundId = Random.Range(0, 4);

                PresentationReferenceHolder.VFXController.SpawnParticleEffect(VFX.HitEffect, new Vector3(x, 0f, z));
                SoundSystem<Sound>.Play((Sound)soundId, new Vector3(x, 0f, z));
            }*/
        }

        public static void GameplayOnExit()
        {
            if (CoreData.IsMultiplayer)
                NetworkManager.Singleton.Shutdown();
        }

        public static void Movement(Vector2 movementInput)
        {
            if (CoreData.IsMultiplayer)
                PresentationData.NetworkPlayers[CoreData.PlayerId!.Value].Move(movementInput.normalized);
            else
                PresentationData.Player.Move(movementInput.normalized);
        }

        public static void SetMusicVolume(int music)
        {
            Assert.IsTrue(music is >= 0 and <= 10, "Volume must be represented by a value randing from 0 to 10.");
            MusicSystem.Volume = music;
        }

        public static void SetSoundVolume(int sound)
        {
            Assert.IsTrue(sound is >= 0 and <= 10, "Volume must be represented by a value randing from 0 to 10.");
            SoundSystem.Volume = sound;
        }

        public static void PlaySound(Sound sound) => SoundSystem.Play(sound);

        public static void SaveGame(BinaryWriter writer)
        {
            Transform player = PresentationData.Player.transform;

            SaveLoadUtils.Write(writer, player.position);
            SaveLoadUtils.Write(writer, player.rotation);
        }

        /// <summary>
        /// If player instance already exists it simply moves it to the spawn point.
        /// </summary>
        static void SpawnSinglePlayer(Transform spawnPoint)
        {
            if (PresentationData.Player == null)
            {
                PlayerView player = Object.Instantiate(
                    _playerConfig.PlayerPrefab, spawnPoint.position, spawnPoint.rotation, PresentationSceneReferenceHolder.PlayerContainer);
                PresentationData.Player = player;
            }
            else
            {
                Transform transform = PresentationData.Player.transform;
                transform.position = spawnPoint.position;
                transform.rotation = spawnPoint.rotation;
            }
        }
    }
}
