using System;
using Shared.DependencyInjector.Atributes;

namespace Shared.Pooling
{
    [NoReflectionBaking]
    public class MemoryPool<TValue> : AbstractMemoryPool<TValue> where TValue : class, new()
    {
        Action<TValue> _onSpawnMethod;

        protected Action<TValue> OnSpawnMethod { set => _onSpawnMethod = value; }

        public MemoryPool(Action<TValue> onSpawnMethod = null, Action<TValue> onDespawnedMethod = null)
            : base(onDespawnedMethod) =>
            _onSpawnMethod = onSpawnMethod;

        public TValue Spawn()
        {
#if ZEN_MULTITHREADING
            lock (_locker)
#endif
            {
                TValue item = SpawnInternal();

                _onSpawnMethod?.Invoke(item);

                return item;
            }
        }

        protected override TValue Alloc() => new();
    }
}