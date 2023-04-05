#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace Common.Views
{
	[CustomEditor(typeof(CommonDebugView))]
	public class CommonDebugViewEditor : Editor
	{
		int value = 5;

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

				if(command.parameters == true)
					value = EditorGUILayout.IntField(value);

				if (GUILayout.Button(commandName))
					command.action?.Invoke(command.parameters ? value : 0);
			}
		}
	}
}
#endif