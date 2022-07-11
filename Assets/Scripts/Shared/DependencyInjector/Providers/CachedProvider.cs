using System;
using System.Collections.Generic;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.Injection;
using Shared.DependencyInjector.Interfaces;
using Shared.DependencyInjector.Internal;

namespace Shared.DependencyInjector.Providers
{
    [NoReflectionBaking]
    class CachedProvider : IProvider
    {
        readonly IProvider _creator;

        List<object> _instances;

#if ZEN_MULTITHREADING
        readonly object _locker = new object();
#endif

        public CachedProvider(IProvider creator) => _creator = creator;

        public Type GetInstanceType(InjectContext context) => _creator.GetInstanceType(context);

        public void GetAllInstancesWithInjectSplit(InjectContext context, out Action injectAction, List<object> buffer)
        {
#if ZEN_MULTITHREADING
            lock (_locker)
#endif
            {
                if (_instances != null)
                {
                    injectAction = null;
                    buffer.AllocFreeAddRange(_instances);
                    return;
                }

                var instances = new List<object>();
                _creator.GetAllInstancesWithInjectSplit(context, out injectAction, instances);
                _instances = instances;
                buffer.AllocFreeAddRange(instances);
            }
        }
    }
}