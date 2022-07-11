using Shared.DependencyInjector.Atributes;
using UnityEngine;

namespace Shared.DependencyInjector.Runtime
{
    class SceneKernel : MonoBehaviour
    {
        [InjectLocal]
        InitializableManager _initializableManager;

        public void Start() => Initialize();

        void Initialize() => _initializableManager.Initialize();
    }
}