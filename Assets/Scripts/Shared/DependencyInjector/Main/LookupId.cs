using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.DataModels;
using Shared.DependencyInjector.Interfaces;
using Shared.DependencyInjector.Providers;

namespace Shared.DependencyInjector.Main
{
    [NoReflectionBaking]
    class LookupId
    {
        internal IProvider Provider;
        internal BindingIdDto BindingIdDto;

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Provider.GetHashCode();
            hash = hash * 23 + BindingIdDto.GetHashCode();
            return hash;
        }

        internal void Reset()
        {
            Provider = null;
            BindingIdDto.Type = null;
        }
    }
}