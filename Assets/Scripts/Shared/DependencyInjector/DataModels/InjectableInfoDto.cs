using System;
using Shared.DependencyInjector.Atributes;

namespace Shared.DependencyInjector.DataModels
{
    [NoReflectionBaking]
    class InjectableInfoDto
    {
        internal readonly bool Optional;
        internal readonly InjectSources SourceType;
        internal readonly string MemberName;
        internal readonly Type MemberType;
        internal readonly object DefaultValue;

        internal InjectableInfoDto(bool optional, string memberName, Type memberType, object defaultValue, InjectSources sourceType)
        {
            Optional = optional;
            MemberType = memberType;
            MemberName = memberName;
            DefaultValue = defaultValue;
            SourceType = sourceType;
        }
    }
}