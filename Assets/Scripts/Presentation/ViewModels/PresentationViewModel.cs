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

        static int _joinedPlayers;

        [Preserve]
        PresentationViewModel() { }

        public void Initialize()
        {
            // this is called for the host too
            NetworkManager.Singleton.OnClientConnectedCallback += clientId =>
            {
                _joinedPlayers++;
                // ignore self connection
                if (clientId == 0)
                    return;

                if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost)
                    return;

                Transform spawnPoint = _level.GetSpawnPoint((PlayerId)(clientId - 1)).transform;
                PlayerNetworkView networkPlayer = Object.Instantiate(_playerConfig.PlayerClientPrefab, spawnPoint.position, spawnPoint.rotation,
                                                              PresentationSceneReferenceHolder.PlayerContainer);

                // this will be assigned only on the host
                PresentationData.NetworkPlayers[(int)(PlayerId)clientId] = networkPlayer;

                // spawn over the network
                networkPlayer.NetworkObj.SpawnWithOwnership(clientId, true);
                networkPlayer.gameObject.SetActive(false);

                // waiting for all players to connect and then display players
                if(NetworkManager.Singleton.IsHost)
                    if (_joinedPlayers != CoreData.PlayerCount)
                        return;

                // show active players
                foreach (PlayerNetworkView player in PresentationData.NetworkPlayers)
                    if (player != null)
                        player.gameObject.SetActive(true);
            };
        }

        public static void CustomUpdate() => _presentationMainController.CustomUpdate();

        public static void CustomFixedUpdate() => _presentationMainController.CustomFixedUpdate();

        public static void CustomLateUpdate() => _presentationMainController.CustomLateUpdate();

        public static void OnCoreSceneLoaded() => PresentationMainController.OnCoreSceneLoaded();

        /// <summary>
        /// This is where network related things happens.
        /// Spawns all players.
        /// </summary>
        public static void OnLevelSceneLoaded()
        {
            // load level data
            _level = GameObject.FindWithTag("LevelSceneReferenceHolder").GetComponent<LevelSceneReferenceHolder>();

            if (CoreData.IsMultiplayer)
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
                        player.gameObject.SetActive(CoreData.PlayerCount == 1);
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
                Vector3 position = SaveLoadUtils.ReadVector3(CoreData.SaveGameReader!);
                Quaternion rotation = SaveLoadUtils.ReadQuaternion(CoreData.SaveGameReader!);
                Transform transform = PresentationData.Player.transform;
                transform.position = position;
                transform.rotation = rotation;
            }
        }

        public static void BootingOnExit() { }

        public static void MainMenuOnEntry()
        {
            MusicSystem.LoadAndPlayWhenReady(Music.MainMenu);
            PresentationSceneReferenceHolder.GameplayCamera.gameObject.SetActive(false);
            PresentationSceneReferenceHolder.MainMenuCamera.gameObject.SetActive(true);
        }

        public static void MainMenuOnExit() => MusicSystem.Stop();

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
            if (CoreData.IsMultiplayer)
            {
                if (NetworkManager.Singleton.IsHost)
                    PresentationData.NetworkPlayers[(int)PlayerId.Player1].Move(movementInput.normalized);
                else
                    PresentationData.NetworkPlayers[(int)CoreData.PlayerId!].Move(movementInput.normalized);
            }
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
