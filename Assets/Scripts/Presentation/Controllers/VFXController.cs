#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
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

        static readonly ObjectPool<ParticleEffectView>[] _pools = new ObjectPool<ParticleEffectView>[Enum.GetNames(typeof(VFX)).Length];
        static readonly List<ParticleEffectView> _particleEffects = new();

        VFX _vfx;
        Vector3 _position;

        [Preserve]
        VFXController() { }

        public void Initialize()
        {
            // for all vfx
            for (int i = 0; i < Enum.GetNames(typeof(VFX)).Length; i++)
                _pools[i] = new ObjectPool<ParticleEffectView>(CustomAlloc, null, CustomReturn);
        }

        public void CustomLateUpdate()
        {
            for (int i = 0; i < _particleEffects.Count; i++)
            {
                ParticleEffectView view = _particleEffects[i];
                if (view.IsAlive)
                    continue;

                _pools[(int)view.VfxType].Return(view);
                _particleEffects.RemoveAt(i);
            }
        }

        internal void SpawnParticleEffect(VFX vfx, Vector3 position)
        {
            _vfx = vfx;
            _position = position;

            ParticleEffectView view = _pools[(int)vfx].Get();
            view.Play();
            _particleEffects.Add(view);
        }

        ParticleEffectView CustomAlloc() => UnityEngine.Object.Instantiate(_config.ParticleEffects[(int)_vfx], _position, Quaternion.identity, PresentationSceneReferenceHolder.VfxContainer);

        static void CustomReturn(ParticleEffectView view) => view.gameObject.SetActive(false);
    }
}