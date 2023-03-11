using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Shared.CheatEngine
{
    public static class DebugCommands
    {
        static readonly List<(Action<int> action, string name, bool parameters, string description, string assembly)> _commands = new();

        /// <summary>
        /// Adds new debug command
        /// </summary>
        /// <param name="action"></param>
        /// <param name="name">Name that appears on the button (in Unity and mobile debugger) or the exact string the user has to type (in console)</param>
        /// <param name="description">Appears only in Unity Editor</param>
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void AddCommand(Action<int> action, string name, bool parameters, string description = null)
        {
            var stackTrace = new StackTrace();
            // get name of the calling assembly one levels above
            // ReSharper disable once PossibleNullReferenceException
            _commands.Add((action, name, parameters, description, stackTrace.GetFrame(1).GetMethod().DeclaringType.Assembly.GetName().Name));
        }
    }
}