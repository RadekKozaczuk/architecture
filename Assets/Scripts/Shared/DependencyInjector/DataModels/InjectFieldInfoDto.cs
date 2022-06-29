using System.Reflection;
using Shared.DependencyInjector.Atributes;

namespace Shared.DependencyInjector.DataModels
{
    [NoReflectionBaking]
    class InjectFieldInfoDto
    {
        internal readonly FieldInfo FieldInfo;
        internal readonly InjectableInfoDto InjectableInfoDto;

        internal InjectFieldInfoDto(FieldInfo fieldInfo, InjectableInfoDto injectableInfoDto)
        {
            InjectableInfoDto = injectableInfoDto;
            FieldInfo = fieldInfo;
        }
    }
}