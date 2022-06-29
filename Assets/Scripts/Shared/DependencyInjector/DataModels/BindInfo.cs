using System;
using System.Collections.Generic;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.Injection;
using Shared.DependencyInjector.Main;
using Shared.DependencyInjector.Util;

namespace Shared.DependencyInjector.DataModels
{
    [NoReflectionBaking]
    public class BindInfo : IDisposable
    {
        public bool OnlyBindIfNotBound;
        public readonly List<Type> ContractTypes;
        public BindingInheritanceMethods BindingInheritanceMethod;
        public bool NonLazy;
        public BindingCondition Condition;
        public ToChoices ToChoice;
        public readonly List<Type> ToTypes; // Only relevant with ToChoices.Concrete
        public ScopeTypes Scope;
        public Action<InjectContext, object> InstantiatedCallback;

        public BindInfo()
        {
            ContractTypes = new List<Type>();
            ToTypes = new List<Type>();

            Reset();
        }

        public void Dispose()
        {
            ZenPools.DespawnBindInfo(this);
        }

        public void Reset()
        {
            OnlyBindIfNotBound = false;
            ContractTypes.Clear();
            BindingInheritanceMethod = BindingInheritanceMethods.None;
            NonLazy = false;
            Condition = null;
            ToChoice = ToChoices.Self;
            ToTypes.Clear();
            Scope = ScopeTypes.Singleton;
            InstantiatedCallback = null;
        }
    }
}