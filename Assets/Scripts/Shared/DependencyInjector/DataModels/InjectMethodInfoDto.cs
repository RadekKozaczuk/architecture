using System.Reflection;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.Main;

namespace Shared.DependencyInjector.DataModels
{
    [NoReflectionBaking]
    class InjectMethodInfoDto
    {
        internal readonly MethodInfo MethodInfo;
        internal readonly DiContainer.ZenInjectMethod Action;
        internal readonly InjectableInfoDto[] Parameters;

        internal InjectMethodInfoDto(DiContainer.ZenInjectMethod action, InjectableInfoDto[] parameters)
        {
            Parameters = parameters;
            Action = action;
        }
        
        public InjectMethodInfoDto(MethodInfo methodInfo, InjectableInfoDto[] parameters)
        {
            MethodInfo = methodInfo;
            Parameters = parameters;
        }
    }
}