using System;
using System.Collections.Generic;
using Shared.DependencyInjector.DataModels;
using Shared.DependencyInjector.Injection;
using Shared.DependencyInjector.Interfaces;
using Shared.DependencyInjector.Main;
using Shared.Pooling;

namespace Shared.DependencyInjector.Util
{
    // TODO: rename to something more generic (less Zenject'y)
    public static class ZenPools
    {
        static readonly MemoryPool<InjectContext> _contextPool = new();
        static readonly MemoryPool<LookupId> _lookupIdPool = new();
        static readonly MemoryPool<BindInfo> _bindInfoPool = new();
        static readonly MemoryPool<BindStatement> _bindStatementPool = new();

        public static Dictionary<TKey, TValue> SpawnDictionary<TKey, TValue>() => DictionaryPool<TKey, TValue>.Instance.Spawn();

        public static BindStatement SpawnStatement() => _bindStatementPool.Spawn();

        public static void DespawnStatement(BindStatement statement)
        {
            statement.Reset();
            _bindStatementPool.Despawn(statement);
        }

        public static BindInfo SpawnBindInfo() => _bindInfoPool.Spawn();

        public static void DespawnBindInfo(BindInfo bindInfo)
        {
            bindInfo.Reset();
            _bindInfoPool.Despawn(bindInfo);
        }

        public static void DespawnDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary) => DictionaryPool<TKey, TValue>.Instance.Despawn(dictionary);

        internal static LookupId SpawnLookupId(IProvider provider, BindingIdDto bindingIdDto)
        {
            LookupId lookupId = _lookupIdPool.Spawn();

            lookupId.Provider = provider;
            lookupId.BindingIdDto = bindingIdDto;

            return lookupId;
        }

        internal static void DespawnLookupId(LookupId lookupId)
        {
            lookupId.Reset();
            _lookupIdPool.Despawn(lookupId);
        }

        public static List<T> SpawnList<T>() => ListPool<T>.Instance.Spawn();

        public static void DespawnList<T>(List<T> list) => ListPool<T>.Instance.Despawn(list);

        public static void DespawnArray<T>(T[] arr) => ArrayPool<T>.GetPool(arr.Length).Despawn(arr);

        public static T[] SpawnArray<T>(int length) => ArrayPool<T>.GetPool(length).Spawn();

        internal static InjectContext SpawnInjectContext(DiContainer container, Type memberType)
        {
            InjectContext context = _contextPool.Spawn();

            context.Container = container;
            context.MemberType = memberType;

            return context;
        }

        public static void DespawnInjectContext(InjectContext context)
        {
            context.Reset();
            _contextPool.Despawn(context);
        }

        internal static InjectContext SpawnInjectContext(
            DiContainer container, InjectableInfoDto injectableInfoDto, object targetInstance, Type targetType)
        {
            InjectContext context = SpawnInjectContext(container, injectableInfoDto.MemberType);

            context.ObjectType = targetType;
            context.ObjectInstance = targetInstance;
            context.MemberName = injectableInfoDto.MemberName;
            context.Optional = injectableInfoDto.Optional;
            context.SourceType = injectableInfoDto.SourceType;
            context.FallBackValue = injectableInfoDto.DefaultValue;

            return context;
        }
    }
}