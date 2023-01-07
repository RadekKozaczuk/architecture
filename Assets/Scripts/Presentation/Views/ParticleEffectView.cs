using System.Collections.Generic;
using Common.Enums;
using UnityEngine;

namespace Presentation.Views
{
    /// <summary>
    /// Represents an object that contains visual effects done in Unity's Particle System.
    /// </summary>
    [DisallowMultipleComponent]
    class ParticleEffectView : AbstractObjectView
    {
        internal bool IsAlive
        {
            get
            {
                for (int i = 0; i < _particleSystems.Count; i++)
                    if (_particleSystems[i].IsAlive())
                        return true;

                return false;
            }
        }

        internal VFX VfxType;

        [SerializeField]
        List<ParticleSystem> _particleSystems;

        internal void Play()
        {
            gameObject.SetActive(true);

            for (int i = 0; i < _particleSystems.Count; i++)
                _particleSystems[i].Play();
        }

        internal void Reset()
        {
            for (int i = 0; i < _particleSystems.Count; i++)
                _particleSystems[i].Stop();
        }
    }
}