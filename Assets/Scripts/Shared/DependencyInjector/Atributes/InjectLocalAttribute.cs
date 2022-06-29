using System;

namespace Shared.DependencyInjector.Atributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    class InjectLocalAttribute : InjectAttributeBase
    {
        internal InjectLocalAttribute() => Source = InjectSources.Local;
    }
}