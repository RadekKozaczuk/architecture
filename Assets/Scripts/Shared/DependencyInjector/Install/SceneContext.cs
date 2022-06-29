using System.Collections.Generic;
using Shared.DependencyInjector.Main;
using Shared.DependencyInjector.Runtime;
using UnityEngine;

namespace Shared.DependencyInjector.Install
{
    public class SceneContext : MonoBehaviour
    {
        public static readonly List<Installer> Installers = new();

        static DiContainer _container;
        static SceneContext _instance;
        
        public static void Run()
        {
            Initialize();
        }
        
        public void Awake()
        {
            _instance = this;
        }

        static void Initialize()
        {
            _container = new DiContainer(new[] {ProjectContext.Instance.Container});
            _container.Bind(typeof(SceneContext)).To<SceneContext>().FromInstance(_instance);
            _container.Bind(typeof(SceneKernel))
                      .To<SceneKernel>().FromNewComponentOn(_instance.gameObject).AsSingle().NonLazy();
            
            foreach (Installer installer in Installers)
            {
                _container.Inject(installer);
                installer.InstallBindings();
            }
            
            _container.ResolveRoots();
        }
    }
}