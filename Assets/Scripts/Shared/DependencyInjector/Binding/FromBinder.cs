using System;
using System.Collections.Generic;
using System.Linq;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.DataModels;
using Shared.DependencyInjector.Injection;
using Shared.DependencyInjector.Internal;
using Shared.DependencyInjector.Main;
using Shared.DependencyInjector.Providers;
using UnityEngine;

namespace Shared.DependencyInjector.Binding
{
    [NoReflectionBaking]
    public class FromBinder
    {
        protected DiContainer BindContainer { get; }

        protected BindStatement BindStatement { get; }

        List<BindInfo> _secondaryBindInfos;
        protected readonly BindInfo BindInfo;

        protected ScopableBindingFinalizer SubFinalizer { set => BindStatement.BindingFinalizer = value; }

        public FromBinder(BindInfo bindInfo) => BindInfo = bindInfo;

        protected FromBinder(DiContainer bindContainer, BindInfo bindInfo, BindStatement bindStatement)
        {
            BindStatement = bindStatement;
            BindContainer = bindContainer;
            BindInfo = bindInfo;
        }

        public FromBinder FromNewComponentOn(GameObject gameObject)
        {
            SubFinalizer = new ScopableBindingFinalizer(
                BindInfo,
                (container, type) => new AddToExistingGameObjectComponentProvider(gameObject, container, type, BindInfo.InstantiatedCallback));

            return new FromBinder(BindInfo);
        }

        public FromBinder FromMethodUntyped(Func<InjectContext, object> method)
        {
            SubFinalizer = new ScopableBindingFinalizer(BindInfo, (_, _) => new MethodProviderUntyped(method));

            return this;
        }

        public FromBinder AsSingle()
        {
            BindInfo.Scope = ScopeTypes.Singleton;
            return this;
        }

        // Note that this is the default so it's not necessary to call this
        public FromBinder AsTransient()
        {
            BindInfo.Scope = ScopeTypes.Transient;
            return this;
        }

        public void WhenInjectedInto(params Type[] targets) =>
            When(r => targets.Any(x => r.ObjectType != null && r.ObjectType.DerivesFromOrEqual(x)));

        public void WhenInjectedInto<T>() => When(r => r.ObjectType != null && r.ObjectType.DerivesFromOrEqual(typeof(T)));

        public FromBinder NonLazy()
        {
            BindInfo.NonLazy = true;
            return this;
        }

        public FromBinder Lazy()
        {
            BindInfo.NonLazy = false;
            return this;
        }

        internal void CopyIntoAllSubContainers()
        {
            BindInfo.BindingInheritanceMethod = BindingInheritanceMethods.CopyIntoAll;

            if (_secondaryBindInfos == null)
                return;

            foreach (BindInfo secondaryBindInfo in _secondaryBindInfos)
                secondaryBindInfo.BindingInheritanceMethod = BindingInheritanceMethods.CopyIntoAll;
        }

        protected void FromInstanceBase(object instance) =>
            SubFinalizer = new ScopableBindingFinalizer(
                BindInfo, (container, type) => new InstanceProvider(type, instance, BindInfo.InstantiatedCallback));

        void When(BindingCondition condition) => BindInfo.Condition = condition;
    }
}