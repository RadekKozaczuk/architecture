using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shared.DependencyInjector.Internal
{
    public static class ExtensionMethods
    {
        // Return the first item when the list is of length one and otherwise returns default
        public static TSource OnlyOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            return source.Count() > 1 ? default : source.FirstOrDefault();
        }

        // These are more efficient than Count() in cases where the size of the collection is not known
        static bool HasAtLeast<T>(this IEnumerable<T> enumerable, int amount) => enumerable.Take(amount).Count() == amount;

        public static bool HasMoreThan<T>(this IEnumerable<T> enumerable, int amount) => enumerable.HasAtLeast(amount + 1);

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();
        
        public static void AllocFreeAddRange<T>(this IList<T> list, IList<T> items)
        {
            for (int i = 0 ; i < items.Count ; i++)
                list.Add(items[i]);
        }
        
        static readonly Dictionary<Type, bool> _isOpenGenericType = new();
        static readonly Dictionary<Type, bool> _isValueType = new();
        static readonly Dictionary<Type, Type[]> _interfaces = new();

        internal static bool DerivesFrom<T>(this Type a) => DerivesFrom(a, typeof(T));

        // This seems easier to think about than IsAssignableFrom
        static bool DerivesFrom(this Type a, Type b) => b != a && a.DerivesFromOrEqual(b);

        internal static bool DerivesFromOrEqual<T>(this Type a) => DerivesFromOrEqual(a, typeof(T));

        internal static bool DerivesFromOrEqual(this Type a, Type b) => b == a || b.IsAssignableFrom(a);

        internal static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            while (true)
            {
                if (givenType.Interfaces().Any(type => type.IsGenericType && type.GetGenericTypeDefinition() == genericType))
                    return true;

                if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                    return true;

                Type baseType = givenType.BaseType;

                if (baseType == null)
                    return false;

                givenType = baseType;
            }
        }

        internal static bool IsValueType(this Type type)
        {
            if (_isValueType.TryGetValue(type, out bool result))
                return result;
            
            result = type.IsValueType;
            _isValueType[type] = result;

            return result;
        }

        internal static Type[] Interfaces(this Type type)
        {
            if (_interfaces.TryGetValue(type, out Type[] result))
                return result;
            
            result = type.GetInterfaces();
            _interfaces.Add(type, result);

            return result;
        }

        internal static object GetDefaultValue(this Type type) => type.IsValueType() ? Activator.CreateInstance(type) : null;

        internal static bool IsOpenGenericType(this Type type)
        {
            if (_isOpenGenericType.TryGetValue(type, out bool result))
                return result;
            
            result = type.IsGenericType && type == type.GetGenericTypeDefinition();
            _isOpenGenericType[type] = result;

            return result;
        }

        internal static bool HasAttribute(this MemberInfo provider, params Type[] attributeTypes) => provider.AllAttributes(attributeTypes).Any();

        internal static IEnumerable<T> AllAttributes<T>(this MemberInfo provider) where T : Attribute 
            => provider.AllAttributes(typeof(T)).Cast<T>();

        static IEnumerable<Attribute> AllAttributes(this MemberInfo provider, params Type[] attributeTypes)
        {
            Attribute[] allAttributes = Attribute.GetCustomAttributes(provider, typeof(Attribute), true);
            
            return attributeTypes.Length == 0 
                ? allAttributes 
                : allAttributes.Where(a => attributeTypes.Any(x => a.GetType().DerivesFromOrEqual(x)));
        }

        internal static IEnumerable<T> AllAttributes<T>(this ParameterInfo provider) where T : Attribute
        {
            Attribute[] allAttributes = Attribute.GetCustomAttributes(provider, typeof(Attribute), true);
            return allAttributes.Where(a => a.GetType().DerivesFromOrEqual(typeof(T))).Cast<T>();
        }
    }
}