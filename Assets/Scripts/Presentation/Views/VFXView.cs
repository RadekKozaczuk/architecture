using System.Collections.Generic;
using UnityEngine;

namespace Presentation.Views
{
    [DisallowMultipleComponent]
    class VFXView : AbstractObjectView
    {
        [SerializeField]
        List<ParticleSystem> _particleSystems;

        internal void Play()
        {
            for (int i = 0; i < _particleSystems.Count; i++)
                _particleSystems[i].Play();
        }
    }
}