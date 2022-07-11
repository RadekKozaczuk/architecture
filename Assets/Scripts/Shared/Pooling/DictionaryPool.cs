using System.Collections.Generic;
using Shared.DependencyInjector.Internal;

namespace Shared.Pooling
{
    public class DictionaryPool<TKey, TValue> : MemoryPool<Dictionary<TKey, TValue>>
    {
        public static DictionaryPool<TKey, TValue> Instance { get; } = new();

        DictionaryPool()
        {
            OnSpawnMethod = OnSpawned;
            OnDespawnedMethod = OnDespawned;
        }

        static void OnSpawned(Dictionary<TKey, TValue> items) => Assert.True(items.IsEmpty());

        static void OnDespawned(Dictionary<TKey, TValue> items) => items.Clear();
    }
}