using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Scripting;

namespace Boot.Controllers
{
    /// <summary>
    /// This class automatically injects config files to corresponding config containers (classes that ends with
    /// 'ConfigContainer')
    /// and systems (static classes).
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-5)]
    public class ConfigInjector : MonoBehaviour
    {
        [InfoBox(
            "Config Injector automatically injects config files to all ConfigContainers and static classes (systems). "
            + "Debug configs are added from the code.", InfoMessageType.None)]
        [SerializeField]
        List<ScriptableObject> _configs;

        [Preserve]
        public void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _configs.Add(Resources.Load<ScriptableObject>("DebugConfig"));
            _configs.Add(Resources.Load<ScriptableObject>("GameLogicDebugConfig"));
            _configs.Add(Resources.Load<ScriptableObject>("PresentationDebugConfig"));
#endif

            if (_configs.Count == 0)
                return;

            // assertions - duplicated and unused configs
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var duplicates = new Dictionary<ScriptableObject, int>();
            for (int i = 0 ; i < _configs.Count ; i++)
                if (!duplicates.TryGetValue(_configs[i], out int _))
                    duplicates.Add(_configs[i], 1);
                else
                    throw new Exception($"Duplicated config in ConfigInjector: {_configs[i]}");

            bool[] usedConfig = new bool[_configs.Count];
#endif

            Assembly[] assemblies = {Assembly.Load("Common"), Assembly.Load("GameLogic"), Assembly.Load("Presentation"), Assembly.Load("UI")};

            for (int i = 0 ; i < assemblies.Length ; i++)
            {
                Assembly asm = assemblies[i];
                Type[] types = asm.GetTypes();
                for (int j = 0 ; j < types.Length ; j++)
                {
                    Type type = types[j];
                    if (!type.IsAbstract || !type.IsSealed)
                        continue;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    bool isConfigContainer = type.Name.Contains("ConfigContainer");
#endif

                    FieldInfo[] fields = type.GetFields(
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    for (int k = 0 ; k < fields.Length ; k++)
                    {
                        FieldInfo field = fields[k];

                        // IsLiteral determines if its value is written at compile time and not changeable
                        // IsInitOnly determines if the field can be set in the body of the constructor
                        // In C# a field which is readonly keyword would have both true.
                        // But a const field would have only IsLiteral equal to true
                        if (field.IsLiteral && !field.IsInitOnly)
                            continue;

                        ScriptableObject config = _configs.Find(c => c.GetType() == field.FieldType);
                        if (config == null)
                        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                            // config container should only contain configs so if there is no a matching one it is for sure an error
                            if (isConfigContainer)
                                throw new Exception($"Could not find config to inject into field '{field.Name}' in '{type.Name}' class.");
#endif

                            continue;
                        }

                        field.SetValue(type, config);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        int index = _configs.FindIndex(c => c.GetType() == field.FieldType);
                        usedConfig[index] = true;
#endif
                    }
                }
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            bool error = false;
            string errorMsg = "";
            for (int i = 0 ; i < usedConfig.Length ; i++)
                if (usedConfig[i] == false)
                {
                    error = true;
                    errorMsg += _configs[i] + ", ";
                }

            if (error)
                throw new Exception("Unused config/s in ConfigInjector: " + errorMsg.TrimEnd(','));
#endif
        }
    }
}