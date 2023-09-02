using System.IO;
using System.Collections.Generic;
using Common;
using Common.Enums;
using ControlFlow.DependencyInjector.Attributes;
using ControlFlow.DependencyInjector.Interfaces;
using JetBrains.Annotations;
using Presentation.Config;
using Presentation.Controllers;
using Presentation.Views;
using Shared.Systems;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Scripting;

namespace Presentation.ViewModels
{
    [UsedImplicitly]
    public class PresentationViewModel : IInitializable
    {
        static PresentationViewModel _instance;

        static readonly AudioConfig _audioConfig;
        static readonly PlayerConfig _playerConfig;

        [Inject]
        readonly AudioController _audioController;

        [Inject]
        readonly VFXController _vfxController;

        [Inject]
        readonly PresentationMainController _presentationMainController;

        static LevelSceneReferenceHolder _level;

        static List<PlayerNetworkView> _players = new();

        static int _joinedPlayers = 0;

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

                if (clientId == 1 && NetworkManager.Singleton.IsHost)
                {
                    Transform spawnPoint = _level.GetSpawnPoint(PlayerId.Player2).transform;
                    PlayerNetworkView player = Object.Instantiate(_playerConfig.PlayerClientPrefab, spawnPoint.position, spawnPoint.rotation,
                                                PresentationSceneReferenceHolder.PlayerContainer);

                    // this will be assigned only on the host
                    PresentationData.NetworkPlayers[(int)PlayerId.Player2] = player;

                    // spawn over the network
                    player.NetworkObj.SpawnWithOwnership(1, true);
                    _players.Add(player);
                    player.ToggleActive(false);
                }
                if (_joinedPlayers == CommonData.NumberOfPlayers)
                    foreach (PlayerNetworkView player in _players)
                        player.ToggleActive(true);
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
                    // todo: hard coded for now
                {
                    Transform spawnPoint = _level.GetSpawnPoint(PlayerId.Player1).transform;

                    if (PresentationData.NetworkPlayers[(int)PlayerId.Player1] == null)
                    {
                        // instantiate locally
                        // in network context objects can only be spawned in root - we cannot spawn under other objects.
                        PlayerNetworkView player = Object.Instantiate(
                            _playerConfig.PlayerServerPrefab,
                            spawnPoint.position,
                            spawnPoint.rotation,
                            // when we delete this, player not spawning correctly
                            PresentationSceneReferenceHolder.PlayerContainer);

                        // this will be assigned only on the host
                        PresentationData.NetworkPlayers[(int)PlayerId.Player1] = player;

                        // spawn over the network
                        // Spawning in Netcode means to instantiate and/or spawn the object that is synchronized between all clients by the server.
                        // Only server can spawn multiplayer objects.
                        player.NetworkObj.Spawn(true);
                        _players.Add(player);
                        player.ToggleActive(CommonData.NumberOfPlayers == 1);
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

            if (CommonData.LoadRequested)
            {
                Vector3 position = SaveLoadUtils.ReadVector3(CommonData.SaveGameReader);
                Quaternion rotation = SaveLoadUtils.ReadQuaternion(CommonData.SaveGameReader);
                Transform transform = PresentationData.Player.transform;
                transform.position = position;
                transform.rotation = rotation;
            }
        }

        public static void BootingOnExit() { }

        public static void MainMenuOnEntry()
        {
            PresentationReferenceHolder.AudioController.LoadMusic(Music.MainMenu);
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
