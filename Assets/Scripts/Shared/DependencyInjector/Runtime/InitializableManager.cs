using System.Collections.Generic;
using Shared.DependencyInjector.Interfaces;
using UnityEngine;
using UnityEngine.Scripting;

namespace Shared.DependencyInjector.Runtime
{
    class InitializableManager
    {
        readonly List<IInitializable> _initializables;

        [Inject]
        internal InitializableManager([Inject(Optional = true, Source = InjectSources.Local)] List<IInitializable> initializables)
        {
            Debug.Log($"=== DEBUGG === InitializableManager InitializableManager initializables.Count: {initializables.Count}");
            _initializables = initializables;
        }

        [Preserve]
        InitializableManager() { }

        internal void Initialize()
        {
            Debug.Log($"=== DEBUGG === InitializableManager Initialize initializables.Count: {_initializables.Count}");
            foreach (IInitializable initializable in _initializables)
                initializable.Initialize();
        }
    }
}