using System;
using System.Collections.Generic;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.DataModels;
using Shared.DependencyInjector.Main;
using Shared.DependencyInjector.Providers;

namespace Shared.DependencyInjector.Binding
{
    [NoReflectionBaking]
    public class ConcreteBinderNonGeneric : FromBinder
    {
        internal ConcreteBinderNonGeneric(DiContainer bindContainer, BindInfo bindInfo, BindStatement bindStatement)
            : base(bindContainer, bindInfo, bindStatement) =>
            ToSelf();

        // Note that this is the default, so not necessary to call
        public ConcreteBinderNonGeneric ToSelf()
        {
            SubFinalizer = new ScopableBindingFinalizer(
                BindInfo, (container, type) => new TransientProvider(type, container, BindInfo.InstantiatedCallback));

            return this;
        }

        public ConcreteBinderNonGeneric To<TConcrete>() => To(typeof(TConcrete));

        public ConcreteBinderNonGeneric To(params Type[] concreteTypes) => To((IEnumerable<Type>)concreteTypes);

        public void FromInstance(object instance) => FromInstanceBase(instance);

        ConcreteBinderNonGeneric To(IEnumerable<Type> concreteTypes)
        {
            BindInfo.ToChoice = ToChoices.Concrete;
            BindInfo.ToTypes.Clear();
            BindInfo.ToTypes.AddRange(concreteTypes);

            return this;
        }
    }
}