#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Shared.CheatEngine;
using System.Reflection;
using System.Linq;

namespace Common.Views {
	[CustomEditor(typeof(CommonDebugView))]
	public class CommonDebugViewEditor : Editor {

		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			CommonDebugView debugView = target as CommonDebugView;

			foreach ((Action<int> action, string name, bool parameters, string description, string assembly) command in debugView.SupportedCommands()) {
				GUILayout.Label(command.description);
				if (GUILayout.Button(command.name)) {
					command.action?.Invoke(0);
				}
			}
		}
	}
}
#endif