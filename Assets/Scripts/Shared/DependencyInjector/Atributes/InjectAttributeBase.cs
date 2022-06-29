using System;
using JetBrains.Annotations;

namespace Shared.DependencyInjector.Atributes
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public abstract class InjectAttributeBase : Attribute
    {
        public bool Optional;
        public InjectSources Source;
    }
}