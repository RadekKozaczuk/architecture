using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ControlFlow.DependencyInjector;
using ControlFlow.DependencyInjector.Attributes;
using UnityEngine;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using ControlFlow.Systems;
#endif
#if UNITY_EDITOR
using ControlFlow.Cli;
#endif

namespace Shared.Systems
{
    /// <summary>
    /// This class automatically injects all controllers, reference holders and viewmodels into corresponding fields decorated with
    /// <see cref="InjectAttribute" />.
    /// All viewmodels must be in 'ViewModels' folder. All controllers in 'Controllers' folder(s), all reference holders in the root folder of the assembly.
    /// Additionally the class injects config fields into all field. Config fields must be private readonly static.
    /// </summary>
    public static class Injector
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        static readonly List<Type> _usedConfigs = new();
#endif

        /// <summary>
        /// This class automatically injects all controllers, reference holders and viewmodels into corresponding fields decorated with
        /// <see cref="InjectAttribute" />.
        /// All viewmodels must be in 'ViewModels' folder. All controllers in 'Controllers' folder(s), all reference holders in the root folder of the assembly.
        /// Additionally the class injects config fields into all field. Config fields must be private readonly static.
        /// </summary>
        public static void Run(IEnumerable<string> additionalAssemblies = null)
        {
            var assemblyNames = new List<string>
            {
                "Boot",
                "Common",
                "GameLogic",
                "Presentation",
                "UI"
            };

            if (additionalAssemblies != null)
                assemblyNames.AddRange(additionalAssemblies);

            DiContainer container = new();
            // ReSharper disable once Unity.UnknownResource
            ScriptableObject[] configs = Resources.LoadAll<ScriptableObject>("Configs");

            var assemblies = new Assembly[assemblyNames.Count];
            for (int i = 0; i < assemblyNames.Count; i++)
                assemblies[i] = Assembly.Load(assemblyNames[i]);

            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly asm = assemblies[i];
                Type[] types = asm.GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    Type type = types[j];

                    // filter out some types created by the compiler f.e. "<PrivateImplementationDetails>+__StaticArrayInitTypeSize=12"
                    if (!type.IsClass)
                        continue;

                    if (type.IsEnum || type.IsInterface)
                        continue;

                    // todo: there are some types created by the compiler f.e. "PrivateImplementationDetails" that we want to filter out here
                    // todo: I don't know how to do it and this method is kinda too generic
                    // todo: however it works well in our case because our convention forces to add namespaces everywhere 
                    if (type.Namespace == null)
                        continue;

                    InjectConfigsIntoFields(type, configs);

                    // ignore internal classes, enums etc.
                    if (type.IsNested || type.IsAbstract)
                        continue;

                    // Bind type in the container if it is a controller, viewmodels, or reference holder.
                    string str = type.Name;
                    if (str.EndsWith("Controller") || str.EndsWith("ReferenceHolder") && !str.EndsWith("SceneReferenceHolder"))
                    {
                        container.Bind(type);
                    }
                    else
                    {
                        str = type.Namespace;

                        // ReSharper disable once PossibleNullReferenceException
                        if (str.EndsWith("Controllers") || str.EndsWith("ViewModels"))
                            container.Bind(type);
                    }
                }
            }

#if UNITY_EDITOR
            // Change the default hook folder to a dedicated one.
            // We have to do this because default hook folder is in .git folder which is always omitted
            // and therefore we would not be able to store pre-commit commands on the repo.
            new CliCommand("git", "config core.hooksPath .github/hooks").Execute();

            // todo: we probably need to also run something like this
            new CliCommand("git", "chmod +x .github/hooks/pre-commit").Execute();

            // todo: not sure if this does anything
            // By default git does not pull submodules leaving them in a detached state.
            // This is annoying for developers. Changing this will make this process automatic. 
            new CliCommand("git", "config submodule.recurse true").Execute();
#endif

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            CheckConfigConsistency(configs);
            CodeConsistencyValidator.CodeConsistencyCheck(assemblyNames);
#endif

            container.ResolveRoots();
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        static void CheckConfigConsistency(IReadOnlyList<ScriptableObject> configs)
        {
            bool unusedError = false;
            bool duplicateError = false;

            string unusedErrorMsg = "";
            string duplicateErrorMsg = "";
            string errorMsg = "";

            List<Type> duplicates = configs.GroupBy(x => x.GetType()).Where(g => g.Count() > 1).Select(y => y.Key).ToList();

            if (duplicates.Count > 0)
                duplicateError = true;

            for (int i = 0; i < duplicates.Count; i++)
                if (i == duplicates.Count - 1)
                    duplicateErrorMsg += duplicates[i];
                else
                    duplicateErrorMsg += duplicates[i] + ", ";

            for (int j = 0; j < configs.Count; j++)
            {
                if (_usedConfigs.Contains(configs[j].GetType()))
                    continue;

                unusedError = true;

                if (j == configs.Count - 1)
                    unusedErrorMsg += configs[j].GetType().Name;
                else
                    unusedErrorMsg += configs[j].GetType().Name + ", ";
            }

            if (unusedError)
                errorMsg += $"Unused config(s): {unusedErrorMsg.TrimEnd(',', ' ')}.\n";

            if (duplicateError)
                errorMsg += $"Duplicated config(s): {duplicateErrorMsg.TrimEnd(',', ' ')}.\n";

            if (unusedError || duplicateError)
                throw new Exception(errorMsg
                                    + "To keep the project clean please always remember to either remove unused or duplicated configs, or use them in the code.\n");
        }
#endif

        static void InjectConfigsIntoFields(IReflect type, ScriptableObject[] configs)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                // IsLiteral determines if its value is written at compile time and not changeable.
                // IsInitOnly determines if the field can be set in the body of the constructor.
                // In C# a field which is readonly keyword would have both true.
                // But a const field would have only IsLiteral equal to true.
                if (field.IsLiteral && !field.IsInitOnly)
                    continue;

                // check if the field is a config filed
                if (!field.FieldType.IsSubclassOf(typeof(ScriptableObject)))
                    continue;

                ScriptableObject config = configs.FirstOrDefault(c => c.GetType() == field.FieldType);
                if (config == null)
                    continue;

                field.SetValue(type, config);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (!_usedConfigs.Contains(field.FieldType))
                    _usedConfigs.Add(field.FieldType);
#endif
            }
        }
    }
}