using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shared.DependencyInjector.Atributes;
using Shared.DependencyInjector.Internal;
using UnityEngine;

namespace Shared.DependencyInjector.DataModels
{
    [NoReflectionBaking]
    class ReflectionTypeInfoDto
    {
        internal readonly List<InjectPropertyInfoDto> InjectProperties;
        internal readonly List<InjectFieldInfoDto> InjectFields;
        internal readonly InjectConstructorInfoDto InjectConstructor;
        internal readonly List<InjectMethodInfoDto> InjectMethods;

        static readonly HashSet<Type> _injectAttributeTypes = new() {typeof(InjectAttributeBase)};
        
        internal ReflectionTypeInfoDto(Type type)
        {
            InjectFields = GetFieldInfos(type);
            InjectConstructor = GetConstructorInfo(type);
            InjectMethods = GetMethodInfos(type);
            InjectProperties = GetPropertyInfos(type);
        }

        static List<InjectPropertyInfoDto> GetPropertyInfos(Type type)
        {
            PropertyInfo[] properties = type.GetProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            
            return properties
                   .Where(x => _injectAttributeTypes.Any(a => x.HasAttribute(a)))
                   .Select(x => new InjectPropertyInfoDto(x, GetInjectableInfoForMember(x))).ToList();
        }

        static List<InjectFieldInfoDto> GetFieldInfos(Type type)
        {
            FieldInfo[] fields = type.GetFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            
            return fields
                   .Where(x => _injectAttributeTypes.Any(a => x.HasAttribute(a)))
                   .Select(
                       x => new InjectFieldInfoDto(x, GetInjectableInfoForMember(x)))
                   .ToList();
        }

        static List<InjectMethodInfoDto> GetMethodInfos(Type type)
        {
            var injectMethodInfos = new List<InjectMethodInfoDto>();

            MethodInfo[] methods = type.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            
            IEnumerable<MethodInfo> methodInfos = methods
                .Where(x => _injectAttributeTypes.Any(a => x.GetCustomAttributes(a, false).Any()));

            foreach (MethodInfo methodInfo in methodInfos)
            {
                InjectableInfoDto[] injectParamInfos = methodInfo.GetParameters().Select(CreateInjectableInfoForParam).ToArray();
                injectMethodInfos.Add(new InjectMethodInfoDto(methodInfo, injectParamInfos));
            }
            
            return injectMethodInfos;
        }

        static InjectConstructorInfoDto GetConstructorInfo(Type type)
        {
            ConstructorInfo constructor = TryGetInjectConstructor(type);

            InjectableInfoDto[] args = constructor == null 
                ? Array.Empty<InjectableInfoDto>() 
                : constructor.GetParameters().Select(CreateInjectableInfoForParam).ToArray();
            return new InjectConstructorInfoDto(constructor, args);
        }

        static InjectableInfoDto CreateInjectableInfoForParam(ParameterInfo paramInfo)
        {
            IEnumerable<InjectAttributeBase> injectAttributes = paramInfo.AllAttributes<InjectAttributeBase>();
            InjectAttributeBase injectAttr = injectAttributes.SingleOrDefault();

            bool isOptional = false;
            var sourceType = InjectSources.Any;

            if (injectAttr != null)
            {
                isOptional = injectAttr.Optional;
                sourceType = injectAttr.Source;
            }

            bool isOptionalWithADefaultValue = (paramInfo.Attributes & ParameterAttributes.HasDefault) == ParameterAttributes.HasDefault;

            return new InjectableInfoDto(
                isOptionalWithADefaultValue || isOptional,
                paramInfo.Name,
                paramInfo.ParameterType,
                isOptionalWithADefaultValue ? paramInfo.DefaultValue : null,
                sourceType);
        }

        static InjectableInfoDto GetInjectableInfoForMember(MemberInfo memInfo)
        {
            List<InjectAttributeBase> injectAttributes = memInfo.AllAttributes<InjectAttributeBase>().ToList();
            InjectAttributeBase injectAttr = injectAttributes.SingleOrDefault();

            bool isOptional = false;
            var sourceType = InjectSources.Any;

            if (injectAttr != null)
            {
                isOptional = injectAttr.Optional;
                sourceType = injectAttr.Source;
            }

            Type memberType = memInfo is FieldInfo
                ? ((FieldInfo) memInfo).FieldType
                : ((PropertyInfo) memInfo).PropertyType;

            return new InjectableInfoDto(isOptional, memInfo.Name, memberType, null, sourceType);
        }

        static ConstructorInfo TryGetInjectConstructor(Type type)
        {
            if (type.DerivesFromOrEqual<Component>())
                return null;

            if (type.IsAbstract)
                return null;

            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (constructors.IsEmpty())
                return null;

            if (!constructors.HasMoreThan(1))
                return constructors[0];
            
            ConstructorInfo explicitConstructor = (from c in constructors 
                                                   where _injectAttributeTypes.Any(a => c.HasAttribute(a)) 
                                                   select c).SingleOrDefault();

            if (explicitConstructor != null)
                return explicitConstructor;

            // If there is only one public constructor then use that
            // This makes decent sense but is also necessary on WSA sometimes since the WSA generated
            // constructor can sometimes be private with zero parameters
            ConstructorInfo singlePublicConstructor = constructors.Where(x => x.IsPublic).OnlyOrDefault();

            return singlePublicConstructor == null 
                ? constructors.OrderBy(x => x.GetParameters().Length).First()
                : singlePublicConstructor;
        }
    }
}