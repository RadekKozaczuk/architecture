using System;
using System.Collections.Generic;
using UnityEngine;

namespace NavMeshComponents.Extensions
{
    public class NavMeshBuilderState
    {
        public Matrix4x4 worldToLocal;
        public Bounds worldBounds;
        public IEnumerable<GameObject> roots;

        public T GetExtraState<T>() where T : class, new()
        {
            _mExtraState ??= new Dictionary<Type, object>();
            if (!_mExtraState.TryGetValue(typeof(T), out object extra))
                extra = _mExtraState[typeof(T)] = new T();
            return extra as T;
        }

        Dictionary<Type, object> _mExtraState;
    }
}