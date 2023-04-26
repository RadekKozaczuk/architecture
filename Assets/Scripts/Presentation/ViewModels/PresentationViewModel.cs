using System.Collections.Generic;
using System.IO;
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

        readonly List<ulong> _clientIds = new();

        [Preserve]
        PresentationViewModel() { }

        public void Initialize()
        {
            _instance = this;

            // this is called for the host too
            NetworkManager.Singleton.OnClientConnectedCallback += clientId =>
            {
                if (CommonData.IsServer)
                {
                    Debug.Log($"ON CLIENT CONNECTED {clientId}");
                    _clientIds.Add(clientId);

                    // we can only give the ownership to the client when the client exists
                    if (clientId == 1)
                    {
                        Debug.Log($"CHANGE OWNERSHIP {_clientIds[1]}");
                        PresentationData.NetworkPlayers[1].NetworkObj.ChangeOwnership(_clientIds[1]);
                    }
                }
            };
        }

        public static void CustomUpdate() => _instance._presentationMainController.CustomUpdate();

        public static void CustomFixedUpdate() => _instance._presentationMainController.CustomFixedUpdate();

        public static void CustomLateUpdate() => _instance._presentationMainController.CustomLateUpdate();

        public static void OnCoreSceneLoaded() => PresentationMainController.OnCoreSceneLoaded();

        /// <summary>
        /// Spawns all players.
        /// </summary>
        public static void OnLevelSceneLoaded()
        {
            // load level data
            var levelSceneReferenceHolder = GameObject.FindWithTag("LevelSceneReferenceHolder").GetComponent<LevelSceneReferenceHolder>();

            Transform container = PresentationSceneReferenceHolder.PlayerContainer;
            Transform spawnPoint;

            if (CommonData.IsMultiplayer)
            {
                if (CommonData.IsClient)
                {
                    NetworkManager.Singleton.StartClient();
                }
                else if (CommonData.IsServer)
                {
                    spawnPoint = levelSceneReferenceHolder.SpawnPoints[0].transform;

                    // spawn locally
                    PlayerNetworkView player = Object.Instantiate(
                        _playerConfig.PlayerServerPrefab, spawnPoint.position, spawnPoint.rotation, container);
                    PresentationData.NetworkPlayers.Add(player);

                    // spawn over the network
                    player.NetworkObj.Spawn(true);

                    // spawn locally
                    player = Object.Instantiate(
                        _playerConfig.PlayerClientPrefab, levelSceneReferenceHolder.SpawnPoints[1].transform.position,
                        Quaternion.identity, container);

                    PresentationData.NetworkPlayers.Add(player);

                    // spawn over the network
                    player.NetworkObj.Spawn(true);
                }
            }
            else
            {
                spawnPoint = levelSceneReferenceHolder.SpawnPoints[0].transform;

                // single player
                PlayerView player = Object.Instantiate(_playerConfig.PlayerPrefab, spawnPoint.position, spawnPoint.rotation, container);
                PresentationData.Player = player;
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

        public static void Movement(Vector2 movementInput)
        {
            if (CommonData.IsMultiplayer)
            {
                List<PlayerNetworkView> players = PresentationData.NetworkPlayers;
                for (int i = 0; i < players.Count; i++)
                {
                    PlayerNetworkView player = players[i];
                    if (player.NetworkObj.IsOwner)
                        player.Move(movementInput.normalized);
                }
            }
            else
            {
                PresentationData.Player.Move(movementInput.normalized);
            }
        }

        public static void SaveGame(BinaryWriter writer)
        {
            Transform player = PresentationData.Player.transform;

            SaveLoadUtils.Write(writer, player.position);
            SaveLoadUtils.Write(writer, player.rotation);
        }
    }
}