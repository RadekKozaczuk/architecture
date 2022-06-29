using Common.Enums;
using Common.Signals;
using JetBrains.Annotations;
using Presentation.Config;
using Shared.DependencyInjector;
using Shared.DependencyInjector.Interfaces;
using Shared.SignalProcessing;
using UnityEngine;
using UnityEngine.Scripting;

namespace Presentation.Controllers
{
    [UsedImplicitly]
    [ReactOnSignals]
    class AudioController : IInitializable
    {
        static readonly AudioConfig _config;

        [Preserve]
        AudioController()
        {
            
        }
        
        public void Initialize()
        {
            SignalProcessor.AddReactiveController(this);
        }

        internal void Play(Sound sound)
        {
            // TODO: should be instantiated in a container
            InstantiateAudioObject(Vector3.zero, _config.Sounds[(int) sound]);
        }

        internal void Play(Music music)
        {
            InstantiateAudioObject(Vector3.zero, _config.Music[(int) music]);
        }

        void Play(Vector3 position)
        {
            AudioClip audioClip = null;

            if (audioClip != null)
                InstantiateAudioObject(position, audioClip);
        }

        // TODO: in future add pooling
        void InstantiateAudioObject(Vector3 position, AudioClip clip)
        {
            AudioSource audioSource = Object.Instantiate(_config.AudioSourcePrefab, position, Quaternion.identity);

            audioSource.clip = clip;
            audioSource.Play();

            Object.Destroy(audioSource.gameObject, audioSource.clip.length);
        }

        [React]
        void OnPlaySound(PlaySoundSignal signal) => Play(signal.Position);
    }
}