#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System.IO;
using Common;
using Common.Enums;
using Common.Systems;
using ControlFlow.DependencyInjector.Attributes;
using ControlFlow.DependencyInjector.Interfaces;
using JetBrains.Annotations;
using Presentation.Config;
using Presentation.Controllers;
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
        public static ScreenOrientation CurrentScreenOrientation
        {
            get
            {
#if UNITY_EDITOR
                if (Screen.height < Screen.width)
                    return ScreenOrientation.LandscapeLeft;
                else
                    return ScreenOrientation.Portrait;
#else
                return Screen.orientation;
#endif
            }
        }

        static PresentationViewModel _instance;

        static readonly PlayerConfig _playerConfig;

        [Inject]
        readonly VFXController _vfxController;

        [Inject]
        readonly PresentationMainController _presentationMainController;

        static LevelSceneReferenceHolder _level;

        static int _joinedPlayers;

        [Preserve]
        PresentationViewModel() { }

        public void Initialize()
        {
            _instance = this;

            // this is called for the host too
            NetworkManager.Singleton.OnClientConnectedCallback += clientId =>
            {
                _joinedPlayers++;
                // ignore self connection
                if (clientId == 0)
                    return;

                if (!NetworkManager.Singleton.IsHost)
                    return;

                if (clientId == 1)
                {
                    Transform spawnPoint = _level.GetSpawnPoint(PlayerId.Player2).transform;
                    PlayerNetworkView player = Object.Instantiate(_playerConfig.PlayerClientPrefab, spawnPoint.position, spawnPoint.rotation,
                                                                  PresentationSceneReferenceHolder.PlayerContainer);

                    // this will be assigned only on the host
                    PresentationData.NetworkPlayers[(int)PlayerId.Player2] = player;

                    // spawn over the network
                    player.NetworkObj.SpawnWithOwnership(1, true);
                    player.gameObject.SetActive(false);
                }

                if (_joinedPlayers != CommonData.PlayerCount)
                    return;

                foreach (PlayerNetworkView player in PresentationData.NetworkPlayers)
                    if (player != null)
                        player.gameObject.SetActive(true);
            };
        }

        public static void CustomUpdate() => _instance._presentationMainController.CustomUpdate();

        public static void CustomFixedUpdate() => _instance._presentationMainController.CustomFixedUpdate();

        public static void CustomLateUpdate() => _instance._presentationMainController.CustomLateUpdate();

        public static void OnCoreSceneLoaded() => PresentationMainController.OnCoreSceneLoaded();

        /// <summary>
        /// This is were network related things happens.
        /// Spawns all players.
        /// </summary>
        public static void OnLevelSceneLoaded()
        {
            // load level data
            _level = GameObject.FindWithTag("LevelSceneReferenceHolder").GetComponent<LevelSceneReferenceHolder>();

            if (CommonData.IsMultiplayer)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    Transform spawnPoint = _level.GetSpawnPoint(PlayerId.Player1).transform;

                    if (PresentationData.NetworkPlayers[(int)PlayerId.Player1] == null)
                    {
                        // instantiate locally
                        // in network context objects can only be spawned in root - we cannot spawn under other objects.
                        PlayerNetworkView player = Object.Instantiate(_playerConfig.PlayerServerPrefab, spawnPoint.position, spawnPoint.rotation);

                        // this will be assigned only on the host
                        PresentationData.NetworkPlayers[(int)PlayerId.Player1] = player;

                        // spawn over the network
                        // Spawning in Netcode means to instantiate and/or spawn the object that is synchronized between all clients by the server.
                        // Only server can spawn multiplayer objects.
                        player.NetworkObj.Spawn(true);
                        player.gameObject.SetActive(CommonData.PlayerCount == 1);
                    }
                    else
                    {
                        Transform transform = PresentationData.NetworkPlayers[(int)PlayerId.Player1].transform;
                        transform.position = spawnPoint.position;
                        transform.rotation = spawnPoint.rotation;
                    }
                }
            }
            else
            {
                SpawnSinglePlayer(_level.GetSpawnPoint(PlayerId.Player1));
            }

            bool loadGameRequested = (bool)GameStateSystem.GetTransitionParameter(StateTransitionParameter.LoadGameRequested)!;
            if (loadGameRequested)
            {
                Vector3 position = SaveLoadUtils.ReadVector3(CommonData.SaveGameReader!);
                Quaternion rotation = SaveLoadUtils.ReadQuaternion(CommonData.SaveGameReader!);
                Transform transform = PresentationData.Player.transform;
                transform.position = position;
                transform.rotation = rotation;
            }
        }

        public static void BootingOnExit() { }

        public static void MainMenuOnEntry()
        {
            MusicSystem<Music>.LoadAndPlayWhenReady(Music.MainMenu);
            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(false);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(true);
        }

        public static void MainMenuOnExit() => MusicSystem<Music>.Stop();

        public static void GameplayOnEntry()
        {
            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(true);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(false);

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

        public static void GameplayOnExit() { }

        public static void Movement(Vector2 movementInput)
        {
            if (CommonData.IsMultiplayer)
            {
                if (NetworkManager.Singleton.IsHost)
                    PresentationData.NetworkPlayers[(int)PlayerId.Player1].Move(movementInput.normalized);
                else
                    PresentationData.NetworkPlayers[(int)PlayerId.Player2].Move(movementInput.normalized);
            }
            else
                PresentationData.Player.Move(movementInput.normalized);
        }

        public static void SetMusicVolume(int music)
        {
            Assert.IsTrue(music is >= 0 and <= 10, "Volume must be represented by a value randing from 0 to 10.");
            MusicSystem<Music>.Volume = music;
        }

        public static void SetSoundVolume(int sound)
        {
            Assert.IsTrue(sound is >= 0 and <= 10, "Volume must be represented by a value randing from 0 to 10.");
            SoundSystem<Sound>.Volume = sound;
        }

        public static void PlaySound(Sound sound) => SoundSystem<Sound>.Play(sound);

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
