using Shared.DependencyInjector.Main;
using Shared.DependencyInjector.Runtime;

namespace Shared.DependencyInjector.Install
{
    public class ProjectContext
    {
        public DiContainer Container { get; private set; }

        public static ProjectContext Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = new ProjectContext();
                _instance.Initialize();

                return _instance;
            }
        }
        static ProjectContext _instance;

        void Initialize()
        {
            Container = new DiContainer(new DiContainer[] { });
            Container.Bind(typeof(InitializableManager)).ToSelf().AsSingle().CopyIntoAllSubContainers();
            Container.ResolveRoots();
        }
    }
}