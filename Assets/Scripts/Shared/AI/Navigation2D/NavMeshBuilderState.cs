using System;
using System.Collections.Generic;
using UnityEngine;

namespace NavMeshComponents.Extensions
{
    public class NavMeshBuilderState
    {
        public Matrix4x4 WorldToLocal;
        public Bounds WorldBounds;
        public IEnumerable<GameObject> Roots;

        Dictionary<Type, object> _extraState;

        public T GetExtraState<T>() where T : class, new()
        {
            _extraState ??= new Dictionary<Type, object>();
            if (!_extraState.TryGetValue(typeof(T), out object extra))
                extra = _extraState[typeof(T)] = new T();
            return extra as T;
        }
    }
}