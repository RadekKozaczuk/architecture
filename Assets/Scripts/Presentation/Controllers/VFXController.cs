using System.Collections.Generic;
using Common.Enums;
using ControlFlow.DependencyInjector.Interfaces;
using ControlFlow.Interfaces;
using ControlFlow.Pooling;
using JetBrains.Annotations;
using Presentation.Config;
using Presentation.Views;
using UnityEngine;
using UnityEngine.Scripting;

namespace Presentation.Controllers
{
    [UsedImplicitly]
    class VFXController : IInitializable, ICustomLateUpdate
    {
        static readonly VFXConfig _config;

        static readonly Dictionary<VFX, MemoryPool<ParticleEffectView>> _poolDictionary = new ();
        static readonly MemoryPool<ParticleEffectView> _particleEffectPool = new();
        static readonly List<ParticleEffectView> _particleEffects = new();

        VFX _vfx;
        Vector3 _position;

        [Preserve]
        VFXController() { }

        public void Initialize()
        {
            // for all vfx
            _poolDictionary.Add(VFX.HitEffect, new MemoryPool<ParticleEffectView>(CustomAlloc, null, CustomReturn));
        }

        public void CustomLateUpdate()
        {
            for (int i = 0; i < _particleEffects.Count; i++)
            {
                ParticleEffectView view = _particleEffects[i];
                if (view.IsAlive)
                    continue;

                _poolDictionary.TryGetValue(view.VfxType, out MemoryPool<ParticleEffectView> pool);

                // ReSharper disable once PossibleNullReferenceException
                pool.Return(view);
                _particleEffects.RemoveAt(i);
            }
        }

        internal void SpawnParticleEffect(VFX vfx, Vector3 position)
        {
            _vfx = vfx;
            _position = position;

            _poolDictionary.TryGetValue(vfx, out MemoryPool<ParticleEffectView> pool);

            // ReSharper disable once PossibleNullReferenceException
            ParticleEffectView view = pool.Get();
            view.Play();
            _particleEffects.Add(view);
        }

        ParticleEffectView CustomAlloc() => Object.Instantiate(_config.ParticleEffects[(int)_vfx], _position, Quaternion.identity, PresentationSceneReferenceHolder.VfxContainer);

        static void CustomReturn(ParticleEffectView view) => view.gameObject.SetActive(false);
    }
}