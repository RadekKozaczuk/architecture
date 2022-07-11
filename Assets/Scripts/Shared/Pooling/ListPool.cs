using System.Collections.Generic;

namespace Shared.Pooling
{
    public class ListPool<T> : MemoryPool<List<T>>
    {
        public static ListPool<T> Instance { get; } = new();
        ListPool() => OnDespawnedMethod = OnDespawned;

        void OnDespawned(List<T> list) => list.Clear();
    }
}