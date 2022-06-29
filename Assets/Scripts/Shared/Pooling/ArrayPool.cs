using System.Collections.Generic;

namespace Shared.Pooling
{
    public class ArrayPool<T> : AbstractMemoryPool<T[]>
    {
        readonly int _length;

        ArrayPool(int length) : base(OnDespawned)
        {
            _length = length;
        }

        static void OnDespawned(T[] arr)
        {
            for (int i = 0 ; i < arr.Length ; i++)
                arr[i] = default(T);
        }

        public T[] Spawn()
        {
#if ZEN_MULTITHREADING
            lock (_locker)
#endif
            {
                return SpawnInternal();
            }
        }

        protected override T[] Alloc() => new T[_length];

        static readonly Dictionary<int, ArrayPool<T>> _pools = new();

        public static ArrayPool<T> GetPool(int length)
        {
            if (_pools.TryGetValue(length, out ArrayPool<T> pool))
                return pool;
            
            pool = new ArrayPool<T>(length);
            _pools.Add(length, pool);

            return pool;
        }
    }
}