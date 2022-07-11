using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.Main;
using Sirenix.Utilities;

namespace Shared.DependencyInjector.DataModels
{
    [NoReflectionBaking]
    class InjectMemberInfoDto
    {
        internal readonly DiContainer.ZenMemberSetterMethod Setter;
        internal readonly InjectableInfoDto InfoDto;

        const BindingFlags FieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        internal InjectMemberInfoDto(InjectFieldInfoDto fieldInfoDto)
        {
            Setter = (injectable, value) => fieldInfoDto.FieldInfo.SetValue(injectable, value);
            InfoDto = fieldInfoDto.InjectableInfoDto;
        }

        internal InjectMemberInfoDto(Type parentType, InjectPropertyInfoDto propertyInfoDto)
        {
            PropertyInfo info = propertyInfoDto.PropertyInfo;
            Setter = info.CanWrite ? (injectable, value) => info.SetValue(injectable, value, null) : GetOnlyPropertySetter(parentType, info.Name);
            InfoDto = propertyInfoDto.InjectableInfoDto;
        }

        static DiContainer.ZenMemberSetterMethod GetOnlyPropertySetter(Type parentType, string propertyName)
        {
            IEnumerable<FieldInfo> allFields = GetAllFields(parentType, FieldFlags);
            IEnumerable<FieldInfo> writeableFields
                = allFields.Where(f => f.Name == string.Format("<" + propertyName + ">k__BackingField", propertyName));

            return (injectable, value) => writeableFields.ForEach(f => f.SetValue(injectable, value));
        }

        static IEnumerable<FieldInfo> GetAllFields(Type t, BindingFlags flags) =>
            t == null ? Enumerable.Empty<FieldInfo>() : t.GetFields(flags).Concat(GetAllFields(t.BaseType, flags)).Distinct();
    }
}