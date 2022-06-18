using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.SignalProcessing.Signals;
using ModestTree;

namespace Common.SignalProcessing
{
    /// <summary>
    /// General purpose signal processing system.
    /// </summary>
    public static class SignalProcessor
    {
        static readonly Type[] _validTypes;
        static readonly Dictionary<Type, MethodInfo[]> _cachedMethodInfos = new();
        static readonly Dictionary<Type, Delegate> _signals = new();

        static SignalProcessor()
        {
            _validTypes = GetValidTypes();
            InitializeReactiveSystems();
            InitializeReactiveControllers();
        }

        public static void AddReactiveController<T>(T controller) where T : class
        {
            if (!_cachedMethodInfos.TryGetValue(typeof(T), out MethodInfo[] methods))
                throw new ArgumentException(
                    $"{typeof(T)} is not a valid controller it needs to implement the ReactOnSignals attribute and at least one [React] method.");

            Assert.False(
                controller == null,
                "Controllers with null values are not allowed to be added to SignalProcessor. "
                + "Maybe you added a variable of certain type before that variable was assigned with a value.");

            for (int i = 0 ; i < methods.Length ; i++)
            {
                MethodInfo method = methods[i];
                Type parameterType = method.GetParameters().First().ParameterType;
                if (_signals.TryGetValue(parameterType, out Delegate action))
                    _signals[parameterType] = Delegate.Combine(
                        action, method.CreateDelegate(typeof(Action<>).MakeGenericType(parameterType), controller));
                else
                    _signals.Add(parameterType, method.CreateDelegate(typeof(Action<>).MakeGenericType(parameterType), controller));
            }
        }

        /// <summary>
        /// Sends a Signal of type <typeparamref name="T" />
        /// <para><typeparamref name="T" /> Needs to be of the explicit type you want send</para>
        /// </summary>
        public static void SendSignal<T>(T abstractSignal) where T : AbstractSignal
        {
            if (!_signals.TryGetValue(typeof(T), out Delegate action))
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                throw new Exception(
                    $"No matching method for the given signal. Signal type: {abstractSignal.GetType()}. Methods marked with [React] should not be public.");
#endif

            (action as Action<T>)?.Invoke(abstractSignal);
        }

        static bool IsStatic(Type type) => type.IsAbstract && type.IsSealed;

        static Type[] GetValidTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
#if ENABLE_MONO
                            .AsParallel()
#endif
                            .SelectMany(assembly => assembly.GetTypes())
                            .Where(t => t.GetCustomAttributes(typeof(ReactOnSignalsAttribute), true).Length > 0)
                            .ToArray();
        }

        static void InitializeReactiveSystems()
        {
            foreach (Type type in _validTypes.Where(IsStatic))
                AddReactiveSystem(type);
        }

        static void InitializeReactiveControllers()
        {
            foreach (Type type in _validTypes.Where(type => !IsStatic(type)))
                CacheControllerMethods(type);
        }

        static void AddReactiveSystem(Type type)
        {
            MethodInfo[] compatibleMethods = type
                                             .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                                             .Where(
                                                 info =>
                                                 {
                                                     if (!info.HasAttribute(typeof(ReactAttribute)))
                                                         return false;
                                                     ParameterInfo[] parameters = info.GetParameters();
                                                     return parameters.Length == 1
                                                            && typeof(AbstractSignal).IsAssignableFrom(parameters[0].ParameterType);
                                                 })
                                             .ToArray();

            for (int i = 0 ; i < compatibleMethods.Length ; i++)
            {
                MethodInfo method = compatibleMethods[i];
                Type parameterType = method.GetParameters().First().ParameterType;
                if (_signals.TryGetValue(parameterType, out Delegate action))
                    _signals[parameterType] = Delegate.Combine(action, method.CreateDelegate(typeof(Action<>).MakeGenericType(parameterType)));
                else
                    _signals.Add(parameterType, method.CreateDelegate(typeof(Action<>).MakeGenericType(parameterType)));
            }
        }

        static void CacheControllerMethods(Type type)
        {
            MethodInfo[] compatibleMethods = type
                                             .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                                             .Where(
                                                 info =>
                                                 {
                                                     if (!info.HasAttribute(typeof(ReactAttribute)))
                                                         return false;
                                                     ParameterInfo[] parameters = info.GetParameters();
                                                     return parameters.Length == 1
                                                            && typeof(AbstractSignal).IsAssignableFrom(parameters[0].ParameterType);
                                                 })
                                             .ToArray();

            if (!compatibleMethods.Any())
                return;

            _cachedMethodInfos.Add(type, compatibleMethods);
        }
    }
}