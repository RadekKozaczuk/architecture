#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Shared.CheatEngine;
using System.Reflection;
using System.Linq;
using ControlFlow.SignalProcessing;
using UnityEngine.Scripting;

namespace Common.Views {
	[CustomEditor(typeof(CommonDebugView))]
	public class CommonDebugViewEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			CommonDebugView debugView = target as CommonDebugView;

			foreach ((Action<int> action, string name, bool parameters, string description, string assembly) command in debugView.SupportedCommands())
			{
				GUILayout.Label(command.description);
				string[] commandWords = command.name.Split("_");
				string commandName= "";
				foreach (string word in commandWords)
				{
					commandName += word + " ";
				}
				if (GUILayout.Button(commandName))
				{
					command.action?.Invoke(0);
				}
			}
		}
	}
}
#endif