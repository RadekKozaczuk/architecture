using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shared.CheatEngine;
using TMPro;
using UnityEngine;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class DebugConsoleView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField]
        TMP_InputField _commandInputField;
        [SerializeField]
        TMP_Text _placeholderText;

        const string CommandsFieldName = "_commands";
        const string PlaceholderDefaultText = "Enter command...";
        const string NoCommandAvailableText = "No command available";
        List<(Action<int> action, string name, bool parameters, string description, string assembly)> _supportedCommands;
        string _currentBestMatch;

		void Start()
        {
			FieldInfo fieldInfo = typeof(DebugCommands).GetFields(BindingFlags.NonPublic | BindingFlags.Static)
                                                       .FirstOrDefault(x => x.Name == CommandsFieldName);

            if (fieldInfo == null)
                return;

            _supportedCommands = (List<(Action<int> action, string name, bool parameters, string description, string assembly)>)fieldInfo.GetValue(null);
			_commandInputField.ActivateInputField();
        }

		internal void UpdatePlaceholderText()
        {
            string currentPlaceholderText = _placeholderText.text;

            if (string.IsNullOrEmpty(_commandInputField.text)
                && string.IsNullOrEmpty(currentPlaceholderText)
                && !_placeholderText.isActiveAndEnabled)
            {
                _placeholderText.text = PlaceholderDefaultText;
                _placeholderText.enabled = true;
                return;
            }

            if (!string.IsNullOrEmpty(_commandInputField.text)
                && !string.IsNullOrEmpty(currentPlaceholderText)
                && currentPlaceholderText != PlaceholderDefaultText)
                _placeholderText.enabled = true;
        }

        internal void TakeBestMatchAsCommand()
        {
            _commandInputField.text = _currentBestMatch;
            _commandInputField.MoveTextEnd(false);
        }

        void CallCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            ManageReceivedCommand(command);

            ClearInputField();
        }

        void ManageReceivedCommand(string command)
        {
            if (!_supportedCommands.Any())
            {
                _placeholderText.text = NoCommandAvailableText;
                return;
            }

            string[] splittedCommand = command.Split(' ');
            command = splittedCommand[0];

            (Action<int> action, string name, bool parameters, string description, string assembly) commandToInvoke = _supportedCommands.FirstOrDefault(x => x.name == command);
            if (commandToInvoke.parameters == true)
            {
                commandToInvoke.action?.Invoke(int.Parse(splittedCommand[1]));
            }
            else
            {
                commandToInvoke.action?.Invoke(0);
            }
        }

        void GetBestMatch(string partOfCommand)
        {
            string placeholderText;

            if (string.IsNullOrEmpty(partOfCommand))
            {
                _currentBestMatch = string.Empty;
                placeholderText = PlaceholderDefaultText;
                _placeholderText.text = placeholderText;

                return;
            }

            List<(Action<int> action, string name, bool parameters, string description, string assembly)> find = _supportedCommands.Where(x => x.name.StartsWith(partOfCommand)).ToList();
            if (find.Count == 1)
            {
                _currentBestMatch = placeholderText = find.First().name;
            }
            else
            {
                (Action<int> action, string name, bool parameters, string description, string assembly) foundCommand = find.FirstOrDefault();
                string currentCommandName = foundCommand.name ?? _supportedCommands.First().name;

                _currentBestMatch = placeholderText = currentCommandName;
            }

            _placeholderText.text = placeholderText;
        }

        void ClearInputField()
        {
            _commandInputField.text = string.Empty;
            _commandInputField.ActivateInputField();
        }
#endif
    }
}