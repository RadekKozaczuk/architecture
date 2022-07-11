using System;
using System.Collections.Generic;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.Injection;
using Shared.DependencyInjector.Interfaces;
using Shared.DependencyInjector.Internal;
using Shared.DependencyInjector.Main;

namespace Shared.DependencyInjector.Providers
{
    [NoReflectionBaking]
    class TransientProvider : IProvider
    {
        readonly DiContainer _container;
        readonly Type _concreteType;
        readonly Action<InjectContext, object> _instantiateCallback;

        internal TransientProvider(Type concreteType, DiContainer container, Action<InjectContext, object> instantiateCallback)
        {
            _container = container;
            _concreteType = concreteType;
            _instantiateCallback = instantiateCallback;
        }

        public Type GetInstanceType(InjectContext context) =>
            !_concreteType.DerivesFromOrEqual(context.MemberType) ? null : GetTypeToCreate(context.MemberType);

        public void GetAllInstancesWithInjectSplit(InjectContext context, out Action injectAction, List<object> buffer)
        {
            Type instanceType = GetTypeToCreate(context.MemberType);

            object instance = _container.InstantiateInternal(instanceType, false);

            injectAction = () =>
            {
                _container.InjectExplicit(instance, instanceType);
                _instantiateCallback?.Invoke(context, instance);
            };

            buffer.Add(instance);
        }

        Type GetTypeToCreate(Type contractType) =>
            _concreteType.IsOpenGenericType() ? _concreteType.MakeGenericType(contractType.GetGenericArguments()) : _concreteType;
    }
}