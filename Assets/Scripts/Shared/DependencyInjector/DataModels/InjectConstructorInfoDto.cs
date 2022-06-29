using System.Reflection;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.Main;

namespace Shared.DependencyInjector.DataModels
{
    [NoReflectionBaking]
    class InjectConstructorInfoDto
    {
        internal readonly DiContainer.ZenFactoryMethod Factory;
        internal readonly ConstructorInfo ConstructorInfo;
        internal readonly InjectableInfoDto[] Parameters;

        internal InjectConstructorInfoDto(DiContainer.ZenFactoryMethod factory, InjectableInfoDto[] parameters)
        {
            Parameters = parameters;
            Factory = factory;
        }
        
        internal InjectConstructorInfoDto(ConstructorInfo constructorInfo, InjectableInfoDto[] parameters)
        {
            ConstructorInfo = constructorInfo;
            Parameters = parameters;
        }
    }
}