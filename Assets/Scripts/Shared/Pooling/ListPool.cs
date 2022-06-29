using System.Collections.Generic;

namespace Shared.Pooling
{
    public class ListPool<T> : MemoryPool<List<T>>
    {
        ListPool() => OnDespawnedMethod = OnDespawned;

        public static ListPool<T> Instance { get; } = new();

        void OnDespawned(List<T> list) => list.Clear();
    }
}