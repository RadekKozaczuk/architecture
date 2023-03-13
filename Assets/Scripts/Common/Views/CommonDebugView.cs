#if UNITY_EDITOR
using Common.Config;
using Common.Signals;
using Shared.Systems;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;
using Shared.CheatEngine;
using System.Linq;

namespace Common.Views
{
    [DisallowMultipleComponent]
    public class CommonDebugView : MonoBehaviour
    {
		const string CommandsFieldName = "_commands";
		List<(Action<int> action, string name, bool parameters, string description, string assembly)> _supportedCommands;
		static readonly DebugConfig _debugConfig;

		public List<(Action<int> action, string name, bool parameters, string description, string assembly)> SupportedCommands()
		{
			return _supportedCommands;
		}

		void Start()
		{
			FieldInfo fieldInfo = typeof(DebugCommands).GetFields(BindingFlags.NonPublic | BindingFlags.Static)
													   .FirstOrDefault(x => x.Name == CommandsFieldName);

			if (fieldInfo == null)
				return;

			_supportedCommands = (List<(Action<int> action, string name, bool parameters, string description, string assembly)>)fieldInfo.GetValue(null);
		}

	}
}
#endif