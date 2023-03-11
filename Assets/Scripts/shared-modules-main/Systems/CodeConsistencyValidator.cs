#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ControlFlow.DependencyInjector.Attributes;
using UnityEngine;

namespace Shared.Systems
{
    /// <summary>
    /// Idea of this class is to help new programmers understand and follow the architecture and code conventions faster
    /// by preventing them from making certain mistakes, and at the same time, providing an explanation why these things are considered mistakes.
    /// Thanks to this we can reduce the time spent on code reviewing.
    /// </summary>
    static class CodeConsistencyValidator
    {
        static readonly List<string> _accessErrors = new();
        static readonly List<string> _injectErrors = new();
        static readonly List<string> _signalErrors = new();
        static readonly List<string> _namingErrors = new();
        static readonly List<string> _genericErrors = new();

        const string AccessErrorMsg = "Reasoning: For better isolation and code readability everything in the assembly should be non public. "
                                      + "With exception for view models (their role is to be the entry point to the assembly), "
                                      + "interface implementations (c# does not allow for internal methods in interfaces), "
                                      + "a and few other things that for various reasons must be public.";

        const string InjectErrorMsg
            = "Reasoning: The reason why fields decorated with [Inject] attribute should be always private and readonly is that:"
              + "\na) static makes no sense as the dependency injector cannot inject into static fields."
              + "\nb) internal and public makes also no sense as we want each class to be injected with what it needs and not have to look into other classes for injectable objects."
              + "\nc) readonly is desired because it is the dependency injection that controls what is assigned to the field not the programmer.";

        const string NamingErrorMsg
            = "Reasoning: To reduce the cognitive complexity of the project all programmers should follow the same conventions:"
              + "\na) internal and public fields should start with uppercase f.e. 'MyFields'"
              + "\nb) private fields should start with an underscore and lowercase f.e. '_myField'\n";

        const string SignalErrorMsg
            = "Reasoning: Signals should be in 'Signals' folder, have 'Signal' suffix and be public, sealed, and inherit from AbstractSignal."
              + "The folder and the suffix is to keep the project orginised. Signals must be public as they have to be able to reach other assemblies."
              + "Sealed is just to inform other programmers that we don't want to have inheritence here as it would only make everything less readable.";

        const BindingFlags BindingFlags = System.Reflection.BindingFlags.DeclaredOnly
                                          | System.Reflection.BindingFlags.Public
                                          | System.Reflection.BindingFlags.Instance;

        internal static void CodeConsistencyCheck(IReadOnlyList<string> assemblyNames)
        {
            for (int i = 0; i < assemblyNames.Count; i++)
            {
                string str = assemblyNames[i];
                Assembly asm;
                switch (str)
                {
                    case "Boot":
                        asm = Assembly.Load(str);
                        ValidateVariables(asm);
                        ValidateSystems(asm, str);
                        ValidateControllers(asm, str);
                        ValidateInjects(asm);
                        break;
                    case "Common":
                        asm = Assembly.Load(str);
                        ValidateVariables(asm);
                        ValidateSignals(asm);
                        ValidateInjects(asm);
                        break;
                    default:
                        asm = Assembly.Load(str);
                        ValidateVariables(asm);
                        ValidateSystems(asm, str);
                        ValidateViews(asm, str);
                        ValidateControllers(asm, str);
                        ValidateInternalData(asm);
                        ValidateInjects(asm);
                        break;
                }
            }

            //DisplayErrorsIfAny();
        }

        /// <summary>
        /// Everything in Signals folder should have "Signal" suffix, be public and sealed, inherit from <see cref="AbstractSignal" />.
        /// </summary>
        static void ValidateSignals(Assembly commonAsm)
        {
            // namespace levels are separated by a dot
            foreach (Type type in GetTypesInNamespace(commonAsm, "Common.Signals"))
            {
                ValidateName(type, "Common", "Signal");

                if (!type.IsPublic)
                    _signalErrors.Add($"{type.Name} should be public.");

                if (!type.IsSealed)
                    _signalErrors.Add($"{type.Name} should be sealed.");

                if (!type.IsSubclassOf(typeof(AbstractSignal)))
                    _signalErrors.Add($"{type.Name} should inherit from AbstractSignal.");
            }
        }

        /// <summary>
        /// Everything in 'Systems' folders should be static.
        /// </summary>
        static void ValidateSystems(Assembly asm, string asmName)
        {
            // TODO: maybe we should also check sub folders or all folders with name 'Systems' across all assemblies
            // TODO: and make the method take an array
            // namespace levels are separated by a dot
            foreach (Type type in GetTypesInNamespace(asm, asmName + ".Systems"))
            {
                // ignore internal classes, enums etc.
                // in this case this allow us to skip occasional static classes' nested classes that compiler creates
                if (type.IsNested)
                    continue;

                if (!ControlFlow.Utils.IsStatic(type))
                    _genericErrors.Add($"Class {type.FullName} is not static. Systems (classes in any 'Systems' folder) should be static. "
                                       + "The idea is that some logic can be packed into classes that does not require instantiation "
                                       + "and if so then we should put these classes in System folder and make them static.");
            }
        }

        /// <summary>
        /// Everything in 'Views' folders should have 'View' suffix and have <see cref="DisallowMultipleComponent" /> attribute. Additionally maybe we should
        /// check if it inherits from a mono at any level of abstraction./>.
        /// </summary>
        static void ValidateViews(Assembly asm, string asmName)
        {
            // namespace levels are separated by a dot
            foreach (Type type in GetTypesInNamespace(asm, asmName + ".Views"))
            {
                // ignore internal classes, enums etc.
                if (type.IsNested)
                    continue;

                ValidateName(type, asmName, "View");

                // inherits from mono on any abstraction level
                if (!typeof(MonoBehaviour).IsAssignableFrom(type))
                    throw new Exception($"Class {type.FullName} does not inherit from MonoBehaviour."
                                        + $"View's (class in {asmName}.Views) purpose in our architecture is to be the main script of the given game object or prefab and therefore it has to inherit directly or indirectly from MonoBehaviour.");

                ValidateAttribute<DisallowMultipleComponent>(
                    type, false,
                    "This is not super important but it helps convey the message that View is the main script of a prefab in our architecture.");
            }
        }

        static void ValidateControllers(Assembly asm, string asmName)
        {
            // namespace levels are separated by a dot 
            foreach (Type type in GetTypesInNamespace(asm, asmName + ".Controllers"))
            {
                // ignore internal classes, enums etc.
                if (type.IsNested)
                    continue;

                if (ControlFlow.Utils.IsStatic(type))
                    throw new Exception($"Class {type.FullName} is static. Controllers should be initializable classes. "
                                        + "The initialization should happen during the dependency injection. "
                                        + $"In order to do that please register the class in the {asmName}Installer.");

                ValidateName(type, asmName, "Controller");

                // that rule does not apply to MainBootController
                if (type.Name != "MainBootController")
                    ValidateAttribute<UsedImplicitlyAttribute>(type, false,
                                                               "Controllers are instantiated via dependency injection (with an exception for MainBootController which is a mono). "
                                                               + "We have to add this attribute to suppress the warning. "
                                                               + "It also helps other programmers to understand the architecture better.");

                // inherits from mono on any abstraction level
                if (typeof(MonoBehaviour).IsAssignableFrom(type))
                    throw new Exception($"Class {type.FullName} should not inherit from MonoBehaviour.");
            }
        }

        static void ValidateName(Type type, string asmName, string prefix)
        {
            // ReSharper disable once PossibleNullReferenceException
            if (type.FullName.EndsWith(prefix))
                return;

            _genericErrors.Add($"Class {type.FullName} does not have '{prefix}' suffix. "
                               + $"{prefix}s (classes in {asmName}.{prefix}s) should have it to better indicate their purpose in the project "
                               + "as well as overall increase code readability (convention).");
        }

        static void ValidateAttribute<T>(Type type, bool allowInherited, string msg) where T : Attribute
        {
            var atr = type.GetCustomAttribute<T>(allowInherited);
            if (atr == null)
                _genericErrors.Add($"Class {type.FullName} does not have {typeof(T).Name} attribute. " + msg);
        }

        static IEnumerable<Type> GetTypesInNamespace(Assembly assembly, string nameSpace) =>
            assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();

        /// <summary>
        /// Everything should be internal except for ViewModels.
        /// </summary>
        static void ValidateInternalData(Assembly asm)
        {
            // namespace levels are separated by a dot 
            Type[] types = asm.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                // ignore internal classes, enums etc.
                if (type.IsNested)
                    continue;

                // if the type is in ViewModel assembly, ignore it
                if (type.Name.Contains("ViewModel") || type.Namespace != null && type.Namespace.Contains("ViewModels"))
                    continue;

                // ignore animation behaviours
                if (type.Name.Contains("AnimationBehaviour") || type.Namespace != null && type.Namespace.Contains("AnimationBehaviours"))
                    continue;

                // ignore debug views
                if (type.Name.Contains("DebugView"))
                    continue;

                // check if the type is internal or less
                if (!type.IsNotPublic)
                    _accessErrors.Add($"Class {type.FullName} should not be public");

                Type[] interfaces = type.GetInterfaces();
                for (int j = 0; j < type.GetMethods(BindingFlags).Length; j++)
                {
                    MethodInfo method = type.GetMethods(BindingFlags)[j];
                    if (method.IsMethodImplementationOfAnyInterface(interfaces))
                        continue;

                    _accessErrors.Add($"Method {method.Name} in {type.FullName} class should not be public.");
                }
            }
        }

        static bool IsMethodImplementationOfAnyInterface(this MethodInfo method, IEnumerable<Type> interfaces)
            // ReSharper disable once PossibleNullReferenceException
            =>
                interfaces.Any(t => method.ReflectedType.GetInterfaceMap(t).TargetMethods.Contains(method));

        static void ValidateInjects(Assembly asm)
        {
            Type[] types = asm.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                FieldInfo[] publicFields = type.GetFields(); // GetFields() by default only returns public fields
                FieldInfo[] nonPublicFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo[] staticFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Static);

                for (int j = 0; j < publicFields.Length; j++)
                {
                    FieldInfo field = publicFields[j];
                    if (field.GetCustomAttribute<InjectAttribute>(false) != null)
                        _injectErrors.Add($"Field {field.Name} in {type.FullName} class should not be public.");
                }

                for (int j = 0; j < staticFields.Length; j++)
                {
                    FieldInfo field = staticFields[j];
                    if (field.GetCustomAttribute<InjectAttribute>(false) != null)
                        _injectErrors.Add($"Field {field.Name} in {type.FullName} class should not be static.");
                }

                for (int j = 0; j < nonPublicFields.Length; j++)
                {
                    FieldInfo field = nonPublicFields[j];
                    if (field.GetCustomAttribute<InjectAttribute>(false) == null)
                        continue;

                    if (!field.IsPrivate && !field.IsInitOnly)
                        _injectErrors.Add($"Field {field.Name} in {type.FullName} class should be private readonly.");
                    else if (!field.IsPrivate)
                        _injectErrors.Add($"Field {field.Name} in {type.FullName} class should be private.");
                    else if (!field.IsInitOnly)
                        _injectErrors.Add($"Field {field.Name} in {type.FullName} class should be readonly.");
                }
            }
        }

        static void ValidateVariables(Assembly asm)
        {
            // namespace levels are separated by a dot
            Type[] types = asm.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                // ignore compiled types and enums
                if (types[i].IsDefined(typeof(CompilerGeneratedAttribute), false) || types[i].IsEnum)
                    continue;

                FieldInfo[] publicFields = types[i].GetFields(BindingFlags.Instance | BindingFlags.Public);
                for (int j = 0; j < publicFields.Length; j++)
                {
                    FieldInfo field = publicFields[j];

                    if (char.IsLower(field.Name[0]) || field.Name[0].Equals('_'))
                        _namingErrors.Add($"Public field '{field.Name}' in {types[i].FullName} class should start with UpperCase.");
                }

                FieldInfo[] privateFields = types[i].GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                for (int j = 0; j < privateFields.Length; j++)
                {
                    FieldInfo field = privateFields[j];

                    if (field.IsAssembly)
                        if (char.IsLower(field.Name[0]) || field.Name[0].Equals('_'))
                            _namingErrors.Add($"Internal field '{field.Name}' in {types[i].FullName} class should start with UpperCase.");

                    if (field.IsPrivate)
                        if (!field.Name[0].Equals('_') || char.IsUpper(field.Name[0]) || char.IsUpper(field.Name[1]))
                            _namingErrors.Add($"Private field '{field.Name}' in {types[i].FullName} class should be named in _camelCase.");
                }
            }
        }

        /// <summary>
        /// Also clears the lists.
        /// </summary>
        static void DisplayErrorsIfAny()
        {
            string msg = "Code validation error. Some parts of the code violate the chosen architectural pattern or the code conventions.";
            bool error = false;

            ParseError(ref error, ref msg, _accessErrors, AccessErrorMsg);
            ParseError(ref error, ref msg, _injectErrors, InjectErrorMsg);
            ParseError(ref error, ref msg, _signalErrors, SignalErrorMsg);
            ParseError(ref error, ref msg, _namingErrors, NamingErrorMsg);
            ParseError(ref error, ref msg, _genericErrors);

            if (error)
                throw new Exception(msg);

            void ParseError(ref bool errorFlag, ref string errorMsg, IList<string> errorList, string reasoning = null)
            {
                if (errorList.Count == 0)
                    return;

                errorFlag = true;

                for (int i = 0; i < errorList.Count; i++)
                {
                    errorMsg += Environment.NewLine;
                    errorMsg += i + 1 + ". " + errorList[i];
                }

                if (reasoning != null)
                    errorMsg += Environment.NewLine + Environment.NewLine + reasoning + Environment.NewLine;

                errorList.Clear();
            }
        }
    }
}
#endif