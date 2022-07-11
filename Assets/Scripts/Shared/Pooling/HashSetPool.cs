using System.Collections.Generic;
using Shared.DependencyInjector.Internal;

namespace Shared.Pooling
{
    public class HashSetPool<T> : MemoryPool<HashSet<T>>
    {
        public static HashSetPool<T> Instance { get; } = new();

        HashSetPool()
        {
            OnSpawnMethod = OnSpawned;
            OnDespawnedMethod = OnDespawned;
        }

        static void OnSpawned(HashSet<T> items) => Assert.True(items.IsEmpty());

        static void OnDespawned(HashSet<T> items) => items.Clear();
    }
}