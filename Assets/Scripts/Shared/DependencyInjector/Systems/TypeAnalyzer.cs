using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shared.DependencyInjector.DataModels;
using Shared.DependencyInjector.Internal;
using Shared.DependencyInjector.Main;
using UnityEngine;

namespace Shared.DependencyInjector.Systems
{
    static class TypeAnalyzer
    {
        delegate InjectTypeInfoDto ZenTypeInfoGetter();

        static readonly Dictionary<Type, InjectTypeInfoDto> _typeInfo = new();

        // Use double underscores for generated methods since this is also what the C# compiler does
        // for things like anonymous methods
        const string ReflectionBakingGetInjectInfoMethodName = "__zenCreateInjectTypeInfo";

        internal static InjectTypeInfoDto TryGetInfo(Type type)
        {
            InjectTypeInfoDto infoDto;

#if ZEN_MULTITHREADING
            lock (_typeInfo)
#endif
            {
                if (_typeInfo.TryGetValue(type, out infoDto))
                    return infoDto;
            }

            infoDto = GetInfoInternal(type);

            if (infoDto != null)
            {
                Type baseType = type.BaseType;

                if (baseType != null && !ShouldSkipTypeAnalysis(baseType))
                    infoDto.BaseTypeInfoDto = TryGetInfo(baseType);
            }

#if ZEN_MULTITHREADING
            lock (_typeInfo)
#endif
            {
                _typeInfo[type] = infoDto;
            }

            return infoDto;
        }

        static InjectTypeInfoDto GetInfoInternal(Type type)
        {
            if (ShouldSkipTypeAnalysis(type))
                return null;

            MethodInfo getInfoMethod = type.GetMethod(ReflectionBakingGetInjectInfoMethodName,
                                                      BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            if (getInfoMethod == null)
                return CreateTypeInfoFromReflection(type);

            var infoGetter = (ZenTypeInfoGetter)Delegate.CreateDelegate(typeof(ZenTypeInfoGetter), getInfoMethod);
            return infoGetter();
        }

        static bool ShouldSkipTypeAnalysis(Type type) =>
            type == null
            || type.IsEnum
            || type.IsArray
            || type.IsInterface
            || type.ContainsGenericParameters
            || IsStaticType(type)
            || type == typeof(object);

        static bool IsStaticType(Type type) =>
            // Apparently this is unique to static classes
            type.IsAbstract && type.IsSealed;

        static InjectTypeInfoDto CreateTypeInfoFromReflection(Type type)
        {
            var reflectionInfo = new ReflectionTypeInfoDto(type);

            DiContainer.ZenFactoryMethod zenFactoryMethod;
            if (type.DerivesFromOrEqual<Component>() || type.IsAbstract)
                zenFactoryMethod = null;
            else
            {
                if (reflectionInfo.InjectConstructor.ConstructorInfo == null)
                    zenFactoryMethod = _ => Activator.CreateInstance(type);
                else
                    zenFactoryMethod = reflectionInfo.InjectConstructor.ConstructorInfo.Invoke;
            }

            var injectConstructor = new InjectConstructorInfoDto(zenFactoryMethod, reflectionInfo.InjectConstructor.Parameters.ToArray());
            InjectMethodInfoDto[] injectMethods = reflectionInfo.InjectMethods.Select(ConvertMethod).ToArray();

            var injectMembers = new InjectMemberInfoDto[reflectionInfo.InjectFields.Count + reflectionInfo.InjectProperties.Count];
            for (int i = 0; i < reflectionInfo.InjectFields.Count; i++)
                injectMembers[i] = new InjectMemberInfoDto(reflectionInfo.InjectFields[i]);
            for (int i = 0; i < reflectionInfo.InjectProperties.Count; i++)
                injectMembers[i] = new InjectMemberInfoDto(type, reflectionInfo.InjectProperties[i]);

            return new InjectTypeInfoDto(injectConstructor, injectMethods, injectMembers);
        }

        static InjectMethodInfoDto ConvertMethod(InjectMethodInfoDto injectMethod)
        {
            void Action(object obj, object[] args)
            {
                injectMethod.MethodInfo.Invoke(obj, args);
            }

            return new InjectMethodInfoDto(Action, injectMethod.Parameters.ToArray());
        }
    }
}