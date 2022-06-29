using System;
using System.Collections.Generic;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.Injection;
using Shared.DependencyInjector.Interfaces;

namespace Shared.DependencyInjector.Providers
{
    [NoReflectionBaking]
    class MethodProviderUntyped : IProvider
    {
        public Type GetInstanceType(InjectContext context) => context.MemberType;
        
        readonly Func<InjectContext, object> _method;

        internal MethodProviderUntyped(Func<InjectContext, object> method)
        {
            _method = method;
        }

        public void GetAllInstancesWithInjectSplit(InjectContext context, out Action injectAction, List<object> buffer)
        {
            injectAction = null;
            object result = _method(context);
            buffer.Add(result);
        }
    }
}