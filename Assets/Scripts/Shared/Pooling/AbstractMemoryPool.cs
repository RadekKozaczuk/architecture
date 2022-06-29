using System;
using System.Collections.Generic;
using Shared.DependencyInjector.Atributes;

namespace Shared.Pooling
{
    [NoReflectionBaking]
    public abstract class AbstractMemoryPool<T> : IDisposable
        where T : class
    {
        // I also tried using ConcurrentBag instead of Stack + lock here but that performed much much worse
        readonly Stack<T> _stack = new();

        // TODO: rename method - despawn does not sound or look good enough
        Action<T> _onDespawnedMethod;
        int _activeCount;

#if ZEN_MULTITHREADING
        protected readonly object _locker = new object();
#endif

        protected AbstractMemoryPool(Action<T> onDespawnedMethod)
        {
            _onDespawnedMethod = onDespawnedMethod;
        }

        protected Action<T> OnDespawnedMethod
        {
            set => _onDespawnedMethod = value;
        }

        public void Dispose()
        {
        }

        // We assume here that we're in a lock
        protected T SpawnInternal()
        {
            T element = _stack.Count == 0 ? Alloc() : _stack.Pop();

            _activeCount++;
            return element;
        }

        public void Despawn(object item)
        {
            Despawn((T) item);
        }

        public void Despawn(T element)
        {
            _onDespawnedMethod?.Invoke(element);

#if ZEN_MULTITHREADING
            lock (_locker)
#endif
            {
                Assert.True(!_stack.Contains(element), "Attempted to despawn element twice!");

                _activeCount--;
                _stack.Push(element);
            }
        }

        protected abstract T Alloc();
    }
}