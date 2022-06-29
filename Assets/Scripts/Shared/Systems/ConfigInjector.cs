using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Shared.Systems
{
    /// <summary>
    /// This class automatically injects config files to corresponding config containers (classes that ends with 'ConfigContainer')
    /// and systems (static classes).
    /// </summary>
    public static class ConfigInjector
    {
        public static void Run(IReadOnlyList<string> assemblyNames)
        {
            ScriptableObject[] configs = Resources.LoadAll<ScriptableObject>("Configs");

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            List<Type> usedConfigs = new();
            CheckConfigConsistency(configs);
#endif

            var assemblies = new Assembly[assemblyNames.Count];
            for (int i = 0 ; i < assemblyNames.Count ; i++)
                assemblies[i] = Assembly.Load(assemblyNames[i]);

            for (int i = 0 ; i < assemblies.Length ; i++)
            {
                Assembly asm = assemblies[i];
                Type[] types = asm.GetTypes();
                for (int j = 0 ; j < types.Length ; j++)
                {
                    Type type = types[j];

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    bool isConfigContainer = type.Name.Contains("ConfigContainer");
#endif

                    FieldInfo[] fields = type.GetFields(
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    
                    for (int k = 0 ; k < fields.Length ; k++)
                    {
                        FieldInfo field = fields[k];

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
                        {
                            // if the scan is not over configs may be found later on
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                            // config container should only contain configs so if there is no a matching one it is for sure an error
                            if (isConfigContainer)
                                throw new Exception($"Could not find config to inject into field '{field.Name}' in '{type.Name}' class.");
#endif

                            continue;
                        }

                        field.SetValue(type, config);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        if (!usedConfigs.Contains(field.FieldType))
                            usedConfigs.Add(field.FieldType);
#endif
                    }
                }
            }

            // TODO: this needs to be synchronized with Zenject to work as intended
            // TODO: for now let's just disable it
/*#if UNITY_EDITOR || DEVELOPMENT_BUILD
            bool error = false;
            string errorMsg = "";
            foreach (ScriptableObject config in configs)
            {
                if (usedConfigs.Contains(config.GetType()))
                    continue;
                
                error = true;
                errorMsg += config.GetType() + ", ";
            }

            if (error)
                throw new Exception($"Unused config(s): {errorMsg.TrimEnd(',')}. Please remove unused configs or use them in the code.");
#endif*/
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // TODO: implement duplicate check and non-instantiated check
        static void CheckConfigConsistency(ScriptableObject[] configs)
        {
            // assertions - duplicated configs

            var duplicates = new Dictionary<ScriptableObject, int>();
            foreach (ScriptableObject config in configs)
                if (duplicates.TryGetValue(config, out int _))
                    throw new Exception($"Duplicated config(s): {config}");
                else
                    duplicates.Add(config, 1);
        }
#endif

    }
}