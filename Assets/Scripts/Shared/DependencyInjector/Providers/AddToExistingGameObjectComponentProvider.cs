using System;
using System.Collections.Generic;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.Injection;
using Shared.DependencyInjector.Interfaces;
using Shared.DependencyInjector.Main;
using UnityEngine;

namespace Shared.DependencyInjector.Providers
{
    [NoReflectionBaking]
    class AddToExistingGameObjectComponentProvider : IProvider
    {
        readonly Action<InjectContext, object> _instantiateCallback;
        readonly DiContainer _container;
        readonly Type _componentType;
        readonly GameObject _gameObject;

        internal AddToExistingGameObjectComponentProvider(GameObject gameObject, DiContainer container, Type componentType, 
            Action<InjectContext, object> instantiateCallback)
        {
            _gameObject = gameObject;
            _componentType = componentType;
            _container = container;
            _instantiateCallback = instantiateCallback;
        }

        public Type GetInstanceType(InjectContext context) => _componentType;

        public void GetAllInstancesWithInjectSplit(InjectContext context, out Action injectAction, List<object> buffer)
        {
            // We still want to make sure we can get the game object during validation

            object instance = _componentType == typeof(Transform) ? _gameObject.transform : _gameObject.AddComponent(_componentType);

            injectAction = () =>
            {
                _container.InjectExplicit(instance, _componentType);
                _instantiateCallback?.Invoke(context, instance);
            };

            buffer.Add(instance);
        }
    }
}