using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.Binding;
using Shared.DependencyInjector.DataModels;
using Shared.DependencyInjector.Injection;
using Shared.DependencyInjector.Interfaces;
using Shared.DependencyInjector.Internal;
using Shared.DependencyInjector.Providers;
using Shared.DependencyInjector.Systems;
using Shared.DependencyInjector.Util;
using UnityEngine;

namespace Shared.DependencyInjector.Main
{
    public delegate bool BindingCondition(InjectContext c);

    // Responsibilities:
    // - Expose methods to configure object graph via BindX() methods
    // - Look up bound values via Resolve() method
    // - Instantiate new values via InstantiateX() methods
    [NoReflectionBaking]
    public class DiContainer
    {
        internal delegate object ZenFactoryMethod(object[] args);
        internal delegate void ZenInjectMethod(object obj, object[] args);
        internal delegate void ZenMemberSetterMethod(object obj, object value);
        
        readonly Dictionary<BindingIdDto, List<ProviderInfoDto>> _providers = new();
        readonly DiContainer[][] _containerLookups = new DiContainer[4][];
        readonly Queue<BindStatement> _currentBindings = new();
        readonly List<BindStatement> _childBindings = new();
        Transform _contextTransform;
        
        internal DiContainer(IEnumerable<DiContainer> parentContainersEnumerable)
        {
            Bind(typeof(DiContainer)).FromInstance(this);
            Bind(typeof(LazyInject)).FromMethodUntyped(CreateLazyBinding).Lazy();
            FlushBindings();

            DiContainer[] selfLookup = {this};
            _containerLookups[(int) InjectSources.Local] = selfLookup;

            DiContainer[] parentContainers = parentContainersEnumerable.ToArray();
            _containerLookups[(int) InjectSources.Parent] = parentContainers;

            DiContainer[] ancestorContainers = FlattenInheritanceChain().ToArray();

            _containerLookups[(int) InjectSources.AnyParent] = ancestorContainers;
            _containerLookups[(int) InjectSources.Any] = selfLookup.Concat(ancestorContainers).ToArray();

            if (parentContainers.IsEmpty())
                return;
            
            for (int i = 0 ; i < parentContainers.Length ; i++)
                parentContainers[i].FlushBindings();

            // Make sure to avoid duplicates which could happen if a parent container
            // appears multiple times in the inheritance chain
            foreach (DiContainer ancestorContainer in ancestorContainers.Distinct())
                foreach (BindStatement binding in ancestorContainer._childBindings)
                    if (ShouldInheritBinding(binding, ancestorContainer))
                        binding.BindingFinalizer.FinalizeBinding(this);
        }

        object CreateLazyBinding(InjectContext context)
        {
            // By cloning it this also means that Ids, optional, etc. are forwarded properly
            InjectContext newContext = context.Clone();
            Type type = context.MemberType.GetGenericArguments().Single();
            newContext.MemberType = type;

            return Activator.CreateInstance(typeof(LazyInject).MakeGenericType(type), this, newContext);
        }

        bool ShouldInheritBinding(BindStatement binding, DiContainer ancestorContainer)
        {
            switch (binding.BindingFinalizer.BindingInheritanceMethod)
            {
                case BindingInheritanceMethods.CopyIntoAll:
                case BindingInheritanceMethods.MoveIntoAll:
                case BindingInheritanceMethods.CopyDirectOnly 
                     or BindingInheritanceMethods.MoveDirectOnly when _containerLookups[(int) InjectSources.Parent].Contains(ancestorContainer):
                    return true;
                default:
                    return false;
            }
        }

        static bool ChecksForCircularDependencies
        {
            get
            {
#if ZEN_MULTITHREADING
                // When multithreading is supported we can't use a static field to track the lookup
                // TODO: We could look at the inject context though
                return false;
#else
                return true;
#endif
            }
        }
        
        public void ResolveRoots()
        {
            FlushBindings();
            ResolveDependencyRoots();
        }

        void ResolveDependencyRoots()
        {
            var rootBindings = new List<BindingIdDto>();
            var rootProviders = new List<ProviderInfoDto>();

            foreach (KeyValuePair<BindingIdDto, List<ProviderInfoDto>> bindingPair in _providers)
                foreach (ProviderInfoDto provider in bindingPair.Value)
                    if (provider.NonLazy)
                    {
                        // Save them to a list instead of resolving for them here to account
                        // for the rare case where one of the resolves does another binding
                        // and therefore changes _providers, causing an exception.
                        rootBindings.Add(bindingPair.Key);
                        rootProviders.Add(provider);
                    }

            List<object> instances = ZenPools.SpawnList<object>();

            try
            {
                for (int i = 0 ; i < rootProviders.Count ; i++)
                {
                    BindingIdDto bindIdDto = rootBindings[i];
                    ProviderInfoDto providerInfoDto = rootProviders[i];

                    using InjectContext context = ZenPools.SpawnInjectContext(this, bindIdDto.Type);
                    context.SourceType = InjectSources.Local;

                    // Should this be true?  Are there cases where you are ok that NonLazy matches
                    // zero providers?
                    // Probably better to be false to catch mistakes
                    context.Optional = false;

                    instances.Clear();
                        
                    SafeGetInstances(providerInfoDto, context, instances);
                }
            }
            finally
            {
                ZenPools.DespawnList(instances);
            }
        }

        public void RegisterProvider(BindingIdDto bindingIdDto, BindingCondition condition, IProvider provider, bool nonLazy)
        {
            var info = new ProviderInfoDto(provider, condition, nonLazy, this);

            if (!_providers.TryGetValue(bindingIdDto, out List<ProviderInfoDto> providerInfos))
            {
                providerInfos = new List<ProviderInfoDto>();
                _providers.Add(bindingIdDto, providerInfos);
            }

            providerInfos.Add(info);
        }

        void GetProviderMatches(InjectContext context, ICollection<ProviderInfoDto> buffer)
        {
            List<ProviderInfoDto> allMatches = ZenPools.SpawnList<ProviderInfoDto>();

            try
            {
                DiContainer[] containerLookups = _containerLookups[(int) context.SourceType];

                for (int i = 0 ; i < containerLookups.Length ; i++)
                    containerLookups[i].FlushBindings();

                for (int i = 0 ; i < containerLookups.Length ; i++)
                    containerLookups[i].GetLocalProviders(context.BindingIdDto, allMatches);
                
                for (int i = 0 ; i < allMatches.Count ; i++)
                {
                    ProviderInfoDto match = allMatches[i];

                    if (match.Condition == null || match.Condition(context))
                        buffer.Add(match);
                }
            }
            finally
            {
                ZenPools.DespawnList(allMatches);
            }
        }

        ProviderInfoDto TryGetUniqueProvider(InjectContext context)
        {
            BindingIdDto bindingIdDto = context.BindingIdDto;
            InjectSources sourceType = context.SourceType;

            DiContainer[] containerLookups = _containerLookups[(int) sourceType];

            for (int i = 0 ; i < containerLookups.Length ; i++)
                containerLookups[i].FlushBindings();

            List<ProviderInfoDto> localProviders = ZenPools.SpawnList<ProviderInfoDto>();

            try
            {
                ProviderInfoDto selected = null;
                int selectedDistance = int.MaxValue;
                bool selectedHasCondition = false;
                bool ambiguousSelection = false;

                for (int i = 0 ; i < containerLookups.Length ; i++)
                {
                    DiContainer container = containerLookups[i];

                    int curDistance = GetContainerHierarchyDistance(container, 0).Value;

                    if (curDistance > selectedDistance)
                        // If matching provider was already found lower in the hierarchy => don't search for a new one,
                        // because there can't be a better or equal provider in this container.
                        continue;

                    localProviders.Clear();
                    container.GetLocalProviders(bindingIdDto, localProviders);

                    for (int k = 0 ; k < localProviders.Count ; k++)
                    {
                        ProviderInfoDto provider = localProviders[k];

                        bool curHasCondition = provider.Condition != null;

                        if (curHasCondition && !provider.Condition(context))
                            // The condition is not satisfied.
                            continue;

                        if (curHasCondition)
                        {
                            ambiguousSelection = selectedHasCondition;
                        }
                        else
                        {
                            if (selectedHasCondition)
                                // Selected provider is better because it has condition.
                                continue;
                            if (selected != null)
                                // Both providers don't have a condition and are on equal depth.
                                ambiguousSelection = true;
                        }

                        if (ambiguousSelection)
                            continue;

                        selectedDistance = curDistance;
                        selectedHasCondition = curHasCondition;
                        selected = provider;
                    }
                }

                return selected;
            }
            finally
            {
                ZenPools.DespawnList(localProviders);
            }
        }

        // Get the full list of ancestor Di Containers, making sure to avoid
        // duplicates and also order them in a breadth-first way
        List<DiContainer> FlattenInheritanceChain()
        {
            var processed = new List<DiContainer>();

            var containerQueue = new Queue<DiContainer>();
            containerQueue.Enqueue(this);

            while (containerQueue.Count > 0)
            {
                DiContainer current = containerQueue.Dequeue();

                foreach (DiContainer parent in current._containerLookups[(int) InjectSources.Parent])
                    if (!processed.Contains(parent))
                    {
                        processed.Add(parent);
                        containerQueue.Enqueue(parent);
                    }
            }

            return processed;
        }

        void GetLocalProviders(BindingIdDto bindingIdDto, IList<ProviderInfoDto> buffer)
        {
            if (_providers.TryGetValue(bindingIdDto, out List<ProviderInfoDto> localProviders))
            {
                buffer.AllocFreeAddRange(localProviders);
                return;
            }

            // If we are asking for a List<int>, we should also match for any localProviders that are bound to the open generic type List<>
            // Currently it only matches one and not the other - not totally sure if this is better than returning both
            if (bindingIdDto.Type.IsGenericType
                && _providers.TryGetValue(new BindingIdDto(bindingIdDto.Type.GetGenericTypeDefinition()), out localProviders))
                buffer.AllocFreeAddRange(localProviders);
        }

        IList ResolveAll(InjectContext context)
        {
            List<object> buffer = ZenPools.SpawnList<object>();

            try
            {
                ResolveAll(context, buffer);
                
                Type genericType = typeof(List<>).MakeGenericType(context.MemberType);

                var list = (IList) Activator.CreateInstance(genericType);

                for (int i = 0 ; i < buffer.Count ; i++)
                {
                    object instance = buffer[i];
                    list.Add(instance);
                }

                return list;
            }
            finally
            {
                ZenPools.DespawnList(buffer);
            }
        }

        void ResolveAll(InjectContext context, IList<object> buffer)
        {
            FlushBindings();
            List<ProviderInfoDto> matches = ZenPools.SpawnList<ProviderInfoDto>();

            try
            {
                GetProviderMatches(context, matches);

                if (matches.Count == 0)
                {
                    return;
                }

                List<object> instances = ZenPools.SpawnList<object>();
                List<object> allInstances = ZenPools.SpawnList<object>();

                try
                {
                    for (int i = 0; i < matches.Count; i++)
                    {
                        ProviderInfoDto match = matches[i];

                        instances.Clear();
                        SafeGetInstances(match, context, instances);

                        for (int k = 0 ; k < instances.Count ; k++)
                            allInstances.Add(instances[k]);
                    }

                    buffer.AllocFreeAddRange(allInstances);
                }
                finally
                {
                    ZenPools.DespawnList(instances);
                    ZenPools.DespawnList(allInstances);
                }
            }
            finally
            {
                ZenPools.DespawnList(matches);
            }
        }

        object Resolve(InjectContext context)
        {
            Type memberType = context.MemberType;
            FlushBindings();
            InjectContext lookupContext = context;

            // The context used for lookups is always the same as the given context EXCEPT for LazyInject<>
            // In CreateLazyBinding above, we forward the context to a new instance of LazyInject<>
            // The problem is, we want the binding for Bind(typeof(LazyInject<>)) to always match even
            // for members that are marked for a specific ID, so we need to discard the identifier
            // for this one particular case
            if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(LazyInject))
            {
                lookupContext = context.Clone();
                lookupContext.SourceType = InjectSources.Local;
                lookupContext.Optional = false;
            }

            ProviderInfoDto providerInfoDto = TryGetUniqueProvider(lookupContext);

            if (providerInfoDto == null)
            {
                // If it's an array try matching to multiple values using its array type
                if (memberType.IsArray && memberType.GetArrayRank() == 1)
                {
                    Type subType = memberType.GetElementType();

                    InjectContext subContext = context.Clone();
                    subContext.MemberType = subType;
                    // By making this optional this means that all injected fields of type T[]
                    // will pass validation, which could be error prone, but I think this is better
                    // than always requiring that they explicitly mark their array types as optional
                    subContext.Optional = true;

                    List<object> results = ZenPools.SpawnList<object>();

                    try
                    {
                        ResolveAll(subContext, results);
                        return CreateArray(subContext.MemberType, results);
                    }
                    finally
                    {
                        ZenPools.DespawnList(results);
                    }
                }

                // If it's a generic list then try matching multiple instances to its generic type
                if (memberType.IsGenericType
                    && (memberType.GetGenericTypeDefinition() == typeof(List<>)
                        || memberType.GetGenericTypeDefinition() == typeof(IList<>)
                        || memberType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    Type subType = memberType.GetGenericArguments().Single();

                    InjectContext subContext = context.Clone();
                    subContext.MemberType = subType;
                    // By making this optional this means that all injected fields of type List<>
                    // will pass validation, which could be error prone, but I think this is better
                    // than always requiring that they explicitly mark their list types as optional
                    subContext.Optional = true;

                    return ResolveAll(subContext);
                }

                if (context.Optional)
                    return context.FallBackValue;
            }

            List<object> instances = ZenPools.SpawnList<object>();

            try
            {
                SafeGetInstances(providerInfoDto, context, instances);

                if (instances.Count == 0 && context.Optional)
                    return context.FallBackValue;

                return instances.First();
            }
            finally
            {
                ZenPools.DespawnList(instances);
            }
        }
        
        static Array CreateArray(Type elementType, IReadOnlyList<object> instances)
        {
            var array = Array.CreateInstance(elementType, instances.Count);

            for (int i = 0 ; i < instances.Count ; i++)
                array.SetValue(instances[i], i);

            return array;
        }

        static void SafeGetInstances(ProviderInfoDto providerInfoDto, InjectContext context, List<object> instances)
        {
            IProvider provider = providerInfoDto.Provider;

            if (ChecksForCircularDependencies)
            {
                LookupId lookupId = ZenPools.SpawnLookupId(provider, context.BindingIdDto);
                provider.GetAllInstancesWithInjectSplit(context, out Action injectAction, instances);
                injectAction?.Invoke();
                ZenPools.DespawnLookupId(lookupId);
            }
            else
            {
                provider.GetAllInstancesWithInjectSplit(context, out Action injectAction, instances);
                injectAction?.Invoke();
            }
        }

        int? GetContainerHierarchyDistance(DiContainer container, int depth)
        {
            if (container == this)
                return depth;

            int? result = null;

            DiContainer[] parentContainers = _containerLookups[(int) InjectSources.Parent];

            for (int i = 0 ; i < parentContainers.Length ; i++)
            {
                int? distance = parentContainers[i].GetContainerHierarchyDistance(container, depth + 1);

                if (distance.HasValue && (!result.HasValue || distance.Value < result.Value))
                    result = distance;
            }

            return result;
        }

        internal object InstantiateInternal(Type concreteType, bool autoInject)
        {
            FlushBindings();
            InjectTypeInfoDto typeInfoDto = TypeAnalyzer.TryGetInfo(concreteType);

            object newObj;

            if (concreteType.DerivesFrom<ScriptableObject>())
            {
                newObj = ScriptableObject.CreateInstance(concreteType);
            }
            else
            {
                // Make a copy since we remove from it below
                object[] paramValues = ZenPools.SpawnArray<object>(typeInfoDto.InjectConstructor.Parameters.Length);

                try
                {
                    for (int i = 0 ; i < typeInfoDto.InjectConstructor.Parameters.Length ; i++)
                    {
                        InjectableInfoDto injectInfoDto = typeInfoDto.InjectConstructor.Parameters[i];

                        using InjectContext subContext = ZenPools.SpawnInjectContext(this, injectInfoDto, null, concreteType);
                        object value = Resolve(subContext);

                        if (value == null)
                            paramValues[i] = injectInfoDto.MemberType.GetDefaultValue();
                        else
                            paramValues[i] = value;
                    }

                    newObj = typeInfoDto.InjectConstructor.Factory(paramValues);
                }
                finally
                {
                    ZenPools.DespawnArray(paramValues);
                }
            }

            if (!autoInject)
                return newObj;
            
            InjectExplicit(newObj, concreteType);

            return newObj;
        }

        public void InjectExplicit(object injectable, Type injectableType)
        {
            InjectTypeInfoDto typeInfoDto = TypeAnalyzer.TryGetInfo(injectableType);

            if (typeInfoDto == null)
                return;

            FlushBindings();

            InjectMembersTopDown(injectable, injectableType, typeInfoDto);
            CallInjectMethodsTopDown(injectable, injectableType, typeInfoDto);
        }

        // When you call any of these Inject methods
        //    Any fields marked [Inject] will be set using the bindings on the container
        //    Any methods marked with a [Inject] will be called
        //    Any constructor parameters will be filled in with values from the container
        internal void Inject(object injectable)
        {
            Type injectableType = injectable.GetType();
            InjectExplicit(injectable, injectableType);
        }

        public bool HasBindingId(Type contractType)
        {
            using InjectContext ctx = ZenPools.SpawnInjectContext(this, contractType);
            ctx.SourceType = InjectSources.Any;
            return HasBinding(ctx);
        }

        // Map the given type to a way of obtaining it
        // Note that this can include open generic types as well such as List<>
        public ConcreteBinderGeneric<TContract> Bind<TContract>()
        {
            BindStatement bindStatement = StartBinding();
            BindInfo bindInfo = bindStatement.SpawnBindInfo();

            bindInfo.ContractTypes.Add(typeof(TContract));

            return new ConcreteBinderGeneric<TContract>(this, bindInfo, bindStatement);
        }

        // Non-generic version of Bind<> for cases where you only have the runtime type
        // Note that this can include open generic types as well such as List<>
        public ConcreteBinderNonGeneric Bind(params Type[] contractTypes)
        {
            BindStatement statement = StartBinding();
            BindInfo bindInfo = statement.SpawnBindInfo();
            bindInfo.ContractTypes.AllocFreeAddRange(contractTypes);
            return BindInternal(bindInfo, statement);
        }
        
        public ConcreteBinderNonGeneric BindInterfacesTo<T>()
        {
            Type type = typeof(T);
            BindStatement statement = StartBinding();
            BindInfo bindInfo = statement.SpawnBindInfo();

            Type[] interfaces = type.Interfaces();

            if (interfaces.Length == 0)
                Debug.Log("Called BindInterfacesTo for type {0} but no interfaces were found");

            bindInfo.ContractTypes.AllocFreeAddRange(interfaces);

            return BindInternal(bindInfo, statement).To(type);
        }
        
        public ConcreteBinderNonGeneric BindInterfacesAndSelfTo<T>()
        {
            Type type = typeof(T);
            BindStatement statement = StartBinding();
            BindInfo bindInfo = statement.SpawnBindInfo();

            bindInfo.ContractTypes.AllocFreeAddRange(type.Interfaces());
            bindInfo.ContractTypes.Add(type);

            return BindInternal(bindInfo, statement).To(type);
        }

        public FromBinder BindInstance<TContract>(TContract instance)
        {
            BindStatement statement = StartBinding();
            BindInfo bindInfo = statement.SpawnBindInfo();
            bindInfo.ContractTypes.Add(typeof(TContract));

            statement.BindingFinalizer = 
                new ScopableBindingFinalizer(bindInfo,
                                             (_, type) => new InstanceProvider(type, instance, bindInfo.InstantiatedCallback));

            return new FromBinder(bindInfo);
        }

        // Unfortunately we can't support setting scope / condition / etc. here since all the
        // bindings are finalized one at a time
        public void BindInstances(params object[] instances)
        {
            for (int i = 0 ; i < instances.Length ; i++)
            {
                object instance = instances[i];
                Bind(instance.GetType()).FromInstance(instance);
            }
        }
        
        // You shouldn't need to use this
        bool HasBinding(InjectContext context)
        {
            FlushBindings();

            List<ProviderInfoDto> matches = ZenPools.SpawnList<ProviderInfoDto>();

            try
            {
                GetProviderMatches(context, matches);
                return matches.Count > 0;
            }
            finally
            {
                ZenPools.DespawnList(matches);
            }
        }

        // You shouldn't need to use this
        void FlushBindings()
        {
            while (_currentBindings.Count > 0)
            {
                BindStatement binding = _currentBindings.Dequeue();

                if (binding.BindingFinalizer.BindingInheritanceMethod != BindingInheritanceMethods.MoveDirectOnly
                    && binding.BindingFinalizer.BindingInheritanceMethod != BindingInheritanceMethods.MoveIntoAll)
                    binding.BindingFinalizer.FinalizeBinding(this);

                if (binding.BindingFinalizer.BindingInheritanceMethod != BindingInheritanceMethods.None)
                    _childBindings.Add(binding);
                else
                    binding.Dispose();
            }
        }

        // Don't use this method
        BindStatement StartBinding()
        {
            FlushBindings();

            BindStatement bindStatement = ZenPools.SpawnStatement();
            _currentBindings.Enqueue(bindStatement);
            return bindStatement;
        }
        
        void CallInjectMethodsTopDown(object injectable, Type injectableType, InjectTypeInfoDto typeInfoDto)
        {
            if (typeInfoDto.BaseTypeInfoDto != null)
                CallInjectMethodsTopDown(injectable, injectableType, typeInfoDto.BaseTypeInfoDto);

            for (int i = 0 ; i < typeInfoDto.InjectMethods.Length ; i++)
            {
                InjectMethodInfoDto method = typeInfoDto.InjectMethods[i];
                object[] paramValues = ZenPools.SpawnArray<object>(method.Parameters.Length);

                try
                {
                    for (int k = 0 ; k < method.Parameters.Length ; k++)
                    {
                        InjectableInfoDto injectInfoDto = method.Parameters[k];
                        using InjectContext subContext = ZenPools.SpawnInjectContext(this, injectInfoDto, injectable, injectableType);
                        paramValues[k] = Resolve(subContext);
                    }

                    method.Action(injectable, paramValues);
                }
                finally
                {
                    ZenPools.DespawnArray(paramValues);
                }
            }
        }

        void InjectMembersTopDown(object injectable, Type injectableType, InjectTypeInfoDto typeInfoDto)
        {
            if (typeInfoDto.BaseTypeInfoDto != null)
                InjectMembersTopDown(injectable, injectableType, typeInfoDto.BaseTypeInfoDto);

            for (int i = 0 ; i < typeInfoDto.InjectMembers.Length ; i++)
            {
                InjectableInfoDto injectInfoDto = typeInfoDto.InjectMembers[i].InfoDto;
                ZenMemberSetterMethod setterMethod = typeInfoDto.InjectMembers[i].Setter;

                object value;
                using (InjectContext subContext = ZenPools.SpawnInjectContext(this, injectInfoDto, injectable, injectableType))
                {
                    value = Resolve(subContext);
                }

                if (!injectInfoDto.Optional || value != null)
                    setterMethod(injectable, value);
            }
        }
        
        ConcreteBinderNonGeneric BindInternal(BindInfo bindInfo, BindStatement bindingFinalizer) => new(this, bindInfo, bindingFinalizer);
    }
}