using System;
using System.Collections.Generic;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.Binding;
using Shared.DependencyInjector.Util;

namespace Shared.DependencyInjector.DataModels
{
    [NoReflectionBaking]
    public class BindStatement : IDisposable
    {
        internal ScopableBindingFinalizer BindingFinalizer;
        readonly List<IDisposable> _disposables= new();

        public BindStatement()
        {
            Reset();
        }

        public BindInfo SpawnBindInfo()
        {
            BindInfo bindInfo = ZenPools.SpawnBindInfo();
            _disposables.Add(bindInfo);
            return bindInfo;
        }
        
        public void Reset()
        {
            BindingFinalizer = null;

            for (int i = 0 ; i < _disposables.Count ; i++)
                _disposables[i].Dispose();

            _disposables.Clear();
        }

        public void Dispose() => ZenPools.DespawnStatement(this);
    }
}