using System.Collections.Generic;
using Common.Enums;
using Common.Signals;
using ControlFlow.Interfaces;
using ControlFlow.Pooling;
using ControlFlow.SignalProcessing;
using JetBrains.Annotations;
using Presentation.Config;
using Shared;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Scripting;

namespace Presentation.Controllers
{
    /// <summary>
    /// Music is loaded in and out of memory dynamically when needed. Sound clips are constantly present in the memory.
    /// </summary>
    [UsedImplicitly]
    [ReactOnSignals]
    class AudioController : ICustomLateUpdate
    {
        static readonly AudioConfig _config;

        readonly AudioClip[] _loadedMusic;
        readonly AsyncOperationHandle<AudioClip>[] _asyncOperationHandles;
        static readonly List<AudioSource> _soundAudioSources = new();
        readonly ObjectPool<AudioSource> _pool;

        Music? _currentMusic;
        Vector3 _position;

        [Preserve]
        AudioController()
        {
            _loadedMusic = new AudioClip[_config.Music.Length];
            _asyncOperationHandles = new AsyncOperationHandle<AudioClip>[_config.Music.Length];
            _pool = new ObjectPool<AudioSource>(CustomAlloc, null, CustomReturn);
        }

        public void CustomLateUpdate()
        {
            for (int i = 0; i < _soundAudioSources.Count; i++)
            {
                AudioSource source = _soundAudioSources[i];
                if (source.isPlaying)
                    continue;

                // todo: add pooling
                _pool.Return(source);
                _soundAudioSources.RemoveAt(i);
            }
        }

        /// <summary>
        /// Loads music asset into memory.
        /// </summary>
        internal void LoadMusic(Music music)
        {
            int id = (int)music;

            Assert.IsNull(_loadedMusic[id], "Request to load music when the music is already loaded into memory is invalid.");

            _asyncOperationHandles[id] = _config.Music[id].LoadAssetAsync<AudioClip>();
            _asyncOperationHandles[id].Completed += asyncOperationHandle => _loadedMusic[id] = asyncOperationHandle.Result;
        }

        /// <summary>
        /// Unloads music asset from memory.
        /// </summary>
        internal void UnloadMusic(Music music)
        {
            int id = (int)music;

            Assert.IsNotNull(_loadedMusic[id],
                             "It is invalid to request to unload a music when the music is already unloaded from memory.");

            _loadedMusic[id] = null;
            _config.Music[id].ReleaseAsset();
        }

        /// <summary>
        /// Tells controller to play the music as soon it is loaded into memory.
        /// If the music is already loaded into memory controller will start to play it immediately.
        /// If the <see cref="LoadMusic"/> method was called but it has not finished yet, controller will wait for it to finish, and then play the music.
        /// If the <see cref="LoadMusic"/> method was not called, controller will call it first, and then wait for it to finish, and then play the music.
        /// </summary>
        internal void PlayMusicWhenReady(Music music)
        {
            int id = (int)music;

            if (_loadedMusic[id] != null)
            {
                HandleMusic();
            }
            else if(_asyncOperationHandles[id].IsDone)
            {
                _asyncOperationHandles[id] = _config.Music[id].LoadAssetAsync<AudioClip>();
                _asyncOperationHandles[id].Completed += asyncOperationHandle =>
                {
                    _loadedMusic[id] = asyncOperationHandle.Result;
                    HandleMusic();
                };
            }
            else
            {
                _asyncOperationHandles[id].Completed += _ => HandleMusic();
            }

            void HandleMusic()
            {
                PresentationSceneReferenceHolder.MusicAudioSource.clip = _loadedMusic[(int)music];
                PresentationSceneReferenceHolder.MusicAudioSource.Play();
                _currentMusic = music;
            }
        }

        /// <summary>
        /// Stops the current music and unloads it from the memory.
        /// </summary>
        internal void StopMusic()
        {
            Assert.IsNotNull(_currentMusic, "Calling StopMusic when no music is playing is invalid.");

            PresentationSceneReferenceHolder.MusicAudioSource.Stop();
            PresentationSceneReferenceHolder.MusicAudioSource.clip = null;

            // ReSharper disable once PossibleInvalidOperationException
            UnloadMusic(_currentMusic.Value);
            _currentMusic = null;
        }

        internal static void PlaySound(Sound sound, Vector3 position)
        {
            AudioSource source = Object.Instantiate(_config.AudioSourcePrefab, position, Quaternion.identity, PresentationSceneReferenceHolder.AudioContainer);
            source.clip = _config.Sounds[(int)sound];
            source.Play();
            _soundAudioSources.Add(source);
        }

        [React]
        [Preserve]
        void OnPlaySound(PlaySoundSignal signal) => PlaySound(signal.SoundType, signal.Position);

        AudioSource CustomAlloc() => Object.Instantiate(_config.AudioSourcePrefab, _position, Quaternion.identity, PresentationSceneReferenceHolder.AudioContainer);

        static void CustomReturn(AudioSource source) => source.gameObject.SetActive(false);
    }
}