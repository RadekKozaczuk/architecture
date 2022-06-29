using System;
using System.Collections.Generic;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.Injection;
using Shared.DependencyInjector.Interfaces;

namespace Shared.DependencyInjector.Providers
{
    [NoReflectionBaking]
    class InstanceProvider : IProvider
    {
        readonly object _instance;
        readonly Type _instanceType;
        readonly Action<InjectContext, object> _instantiateCallback;

        public InstanceProvider(Type instanceType, object instance, Action<InjectContext, object> instantiateCallback)
        {
            _instanceType = instanceType;
            _instance = instance;
            _instantiateCallback = instantiateCallback;
        }

        public Type GetInstanceType(InjectContext context) => _instanceType;

        public void GetAllInstancesWithInjectSplit(InjectContext context, out Action injectAction, List<object> buffer)
        {
            injectAction = () =>
            {
                _instantiateCallback?.Invoke(context, _instance);
            };

            buffer.Add(_instance);
        }
    }
}