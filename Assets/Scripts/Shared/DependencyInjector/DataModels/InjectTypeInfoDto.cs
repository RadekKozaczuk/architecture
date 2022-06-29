using Shared.DependencyInjector.Atributes;

namespace Shared.DependencyInjector.DataModels
{
    [NoReflectionBaking]
    class InjectTypeInfoDto
    {
        internal InjectTypeInfoDto BaseTypeInfoDto;

        internal readonly InjectMethodInfoDto[] InjectMethods;
        internal readonly InjectMemberInfoDto[] InjectMembers;
        internal readonly InjectConstructorInfoDto InjectConstructor;

        internal InjectTypeInfoDto(InjectConstructorInfoDto injectConstructor, InjectMethodInfoDto[] injectMethods, InjectMemberInfoDto[] injectMembers)
        {
            InjectMethods = injectMethods;
            InjectMembers = injectMembers;
            InjectConstructor = injectConstructor;
        }
    }
}