using System;
using Shared.DependencyInjector.Main;

namespace Shared.DependencyInjector.Install
{
    public abstract class Installer
    {
        protected DiContainer Container => _container;

        [Inject]
        DiContainer _container;

        public virtual void InstallBindings() => throw new NotImplementedException();
    }
}