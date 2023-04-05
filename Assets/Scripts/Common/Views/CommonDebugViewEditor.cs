#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace Common.Views
{
	[CustomEditor(typeof(CommonDebugView))]
	public class CommonDebugViewEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			var debugView = target as CommonDebugView;

			foreach ((Action<int> action, string name, bool parameters, string description, string assembly) command in debugView!.SupportedCommands())
			{
				GUILayout.Label(command.description);
				string[] commandWords = command.name.Split("_");
				string commandName = "";

				foreach (string word in commandWords)
					commandName += word + " ";

				if (GUILayout.Button(commandName))
					command.action?.Invoke(0);
			}
		}
	}
}
#endif