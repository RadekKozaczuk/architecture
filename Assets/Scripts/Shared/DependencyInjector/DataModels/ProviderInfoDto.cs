using Shared.DependencyInjector.Interfaces;
using Shared.DependencyInjector.Main;

namespace Shared.DependencyInjector.DataModels
{
    class ProviderInfoDto
    {
        internal readonly bool NonLazy;
        internal readonly IProvider Provider;
        internal readonly BindingCondition Condition;

        internal ProviderInfoDto(IProvider provider, BindingCondition condition, bool nonLazy, DiContainer container)
        {
            Provider = provider;
            Condition = condition;
            NonLazy = nonLazy;
        }
    }
}