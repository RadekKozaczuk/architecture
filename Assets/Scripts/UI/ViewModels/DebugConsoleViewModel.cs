using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common;
using Shared.CheatEngine;
using UnityEngine;

namespace UI.ViewModels
{
    public static class DebugConsoleViewModel
    {
        const string CommandsFieldName = "_commands";

        public static void ManageReceivedCommand(string command)
        {
            var fieldInfo = typeof(DebugCommands).GetFields(BindingFlags.NonPublic | BindingFlags.Static)
                .FirstOrDefault(x => x.Name == CommandsFieldName);
            if (fieldInfo == null)
                return;

            var supportedCommands =
                (List<(Action action, string name, string description, string assembly)>)fieldInfo.GetValue(null);
            if (!supportedCommands.Any())
            {
                CommonDebugCommands.InitializeDebugCommands();

                supportedCommands =
                    (List<(Action action, string name, string description, string assembly)>)fieldInfo.GetValue(null);
            }

            var commandToInvoke = supportedCommands.FirstOrDefault(x => x.name == command);
            if (commandToInvoke.action == null)
            {
                Debug.Log($"Received command: {command} was not found.");
                return;
            }

            Debug.Log($"Calling command named: {commandToInvoke.name}, {commandToInvoke.description}");
            commandToInvoke.action.Invoke();
        }
    }
}