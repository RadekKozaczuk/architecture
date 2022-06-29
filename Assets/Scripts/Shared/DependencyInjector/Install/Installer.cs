using System;
using System.Diagnostics;
using Shared.DependencyInjector.Interfaces;
using Shared.DependencyInjector.Main;
using UnityEngine;

namespace Shared.DependencyInjector.Install
{
    public abstract class Installer
    {
        protected DiContainer Container => _container;

        [Inject]
        DiContainer _container;

        public virtual void InstallBindings()
        {
            throw new NotImplementedException();
        }
    }
}