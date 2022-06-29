using System.Reflection;
using Shared.DependencyInjector.Atributes;

namespace Shared.DependencyInjector.DataModels
{
    [NoReflectionBaking]
    class InjectPropertyInfoDto
    {
        internal readonly PropertyInfo PropertyInfo;
        internal readonly InjectableInfoDto InjectableInfoDto;

        internal InjectPropertyInfoDto(PropertyInfo propertyInfo, InjectableInfoDto injectableInfoDto)
        {
            InjectableInfoDto = injectableInfoDto;
            PropertyInfo = propertyInfo;
        }
    }
}