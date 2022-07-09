using System;
using System.Collections.Generic;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.DataModels;
using Shared.DependencyInjector.Interfaces;
using Shared.DependencyInjector.Internal;
using Shared.DependencyInjector.Main;
using Shared.DependencyInjector.Providers;
using Shared.DependencyInjector.Util;

namespace Shared.DependencyInjector.Binding
{
    [NoReflectionBaking]
    public class ScopableBindingFinalizer
    {
        BindInfo BindInfo { get; }
        internal BindingInheritanceMethods BindingInheritanceMethod => BindInfo.BindingInheritanceMethod;
        readonly Func<DiContainer, Type, IProvider> _providerFactory;

        internal ScopableBindingFinalizer(BindInfo bindInfo, Func<DiContainer, Type, IProvider> providerFactory)
        {
            _providerFactory = providerFactory;
            BindInfo = bindInfo;
        }

        internal void FinalizeBinding(DiContainer container)
        {
            if (BindInfo.ContractTypes.Count == 0)
                return;

            Func<DiContainer, Type, IProvider> providerFunc = BindInfo.Scope == ScopeTypes.Singleton 
                ? (_, type) => new CachedProvider(_providerFactory(container, type)) 
                : _providerFactory;
            
            if (BindInfo.ToChoice == ToChoices.Self)
                RegisterProviderPerContract(container, providerFunc);
            else if (BindInfo.ToTypes.Count != 0)
                RegisterProvidersForAllContractsPerConcreteType(container, BindInfo.ToTypes, providerFunc);
        }

        void RegisterProviderPerContract(DiContainer container, Func<DiContainer, Type, IProvider> providerFunc)
        {
            foreach (Type contractType in BindInfo.ContractTypes)
            {
                IProvider provider = providerFunc(container, contractType);
                RegisterProvider(container, contractType, provider);
            }
        }
        
        void RegisterProvider(DiContainer container, Type contractType, IProvider provider)
        {
            if (BindInfo.OnlyBindIfNotBound && container.HasBindingId(contractType))
                return;

            container.RegisterProvider(new BindingIdDto(contractType), BindInfo.Condition, provider, BindInfo.NonLazy);

            if (!contractType.IsValueType() || contractType.IsGenericType && contractType.GetGenericTypeDefinition() == typeof(Nullable<>))
                return;
            
            Type nullableType = typeof(Nullable<>).MakeGenericType(contractType);

            // Also bind to nullable primitives
            // this is useful so that we can have optional primitive dependencies
            container.RegisterProvider(new BindingIdDto(nullableType), BindInfo.Condition, provider, BindInfo.NonLazy);
        }

        // Returns true if the bind should continue, false to skip
        static bool ValidateBindTypes(Type concreteType, Type contractType)
        {
            bool isConcreteOpenGenericType = concreteType.IsOpenGenericType();
            bool isContractOpenGenericType = contractType.IsOpenGenericType();
            if (isConcreteOpenGenericType != isContractOpenGenericType)
                return false;

#if !(UNITY_WSA && ENABLE_DOTNET)
            // TODO: Is it possible to do this on WSA?

            if (isContractOpenGenericType)
            {
                if (ExtensionMethods.IsAssignableToGenericType(concreteType, contractType))
                    return true;
            }
            else if (concreteType.DerivesFromOrEqual(contractType))
                return true;
#else
            if (concreteType.DerivesFromOrEqual(contractType))
            {
                return true;
            }
#endif

            throw new Exception("Expected type '{0}' to derive from or be equal to '{1}'");
        }

        // Note that if multiple contract types are provided per concrete type,
        // it will re-use the same provider for each contract type
        // (each concrete type will have its own provider though)
        void RegisterProvidersForAllContractsPerConcreteType(DiContainer container, List<Type> concreteTypes, 
            Func<DiContainer, Type, IProvider> providerFunc)
        {
            Dictionary<Type, IProvider> providerMap = ZenPools.SpawnDictionary<Type, IProvider>();
            try
            {
                foreach (Type concreteType in concreteTypes)
                    providerMap[concreteType] = providerFunc(container, concreteType);

                foreach (Type contractType in BindInfo.ContractTypes)
                    foreach (Type concreteType in concreteTypes)
                        if (ValidateBindTypes(concreteType, contractType))
                            RegisterProvider(container, contractType, providerMap[concreteType]);
            }
            finally
            {
                ZenPools.DespawnDictionary(providerMap);
            }
        }
    }
}