using System;
using System.Collections.Generic;
using Shared.DependencyInjector.Injection;

namespace Shared.DependencyInjector.Interfaces
{
    public interface IProvider
    {
        Type GetInstanceType(InjectContext context);

        // Return an instance which might be not yet injected to.
        // injectAction should handle the actual injection
        // This way, providers that call CreateInstance() can store the instance immediately,
        // and then return that if something gets created during injection that refers back
        // to the newly created instance
        void GetAllInstancesWithInjectSplit(InjectContext context, out Action injectAction, List<object> instances);
    }
}