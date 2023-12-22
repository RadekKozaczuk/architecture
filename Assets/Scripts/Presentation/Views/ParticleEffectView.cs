#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#nullable enable
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
            {
                ParticleSystem ps = _particleSystems[i];
                RandomizeColors(ps);
                ps.Play();
            }
        }

        // example of how we can control particles' parameters
        static void RandomizeColors(ParticleSystem ps)
        {
            // todo: something does not work here - it does not change the values
            /*ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.color.gradient.colorKeys[0].color.r = Random.Range(0, 1);
            colorOverLifetime.color.gradient.colorKeys[0].color.g = Random.Range(0, 1);
            colorOverLifetime.color.gradient.colorKeys[0].color.b = Random.Range(0, 1);
            colorOverLifetime.color.gradient.colorKeys[1].color.r = Random.Range(0, 1);
            colorOverLifetime.color.gradient.colorKeys[1].color.g = Random.Range(0, 1);
            colorOverLifetime.color.gradient.colorKeys[1].color.b = Random.Range(0, 1);*/
        }
    }
}