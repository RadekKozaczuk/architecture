#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && UNITY_STANDALONE_WIN
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common;
using Shared.CheatEngine;
using TMPro;
using UI.ViewModels;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class DebugConsoleView : MonoBehaviour
    {
        [SerializeField]
        TMP_InputField _commandInputField;
        [SerializeField] 
        TMP_Text _placeholderText;
        [SerializeField]
        GameObject _debugConsole;

        const string CommandsFieldName = "_commands";
        const string PlaceholderDefaultText = "Enter command...";
        const string NoCommandAvailableText = "No command available";
        List<(Action action, string name, string description, string assembly)> _supportedCommands;
        string _currentBestMatch;

        void Awake()
        {
            var fieldInfo = typeof(DebugCommands).GetFields(BindingFlags.NonPublic | BindingFlags.Static)
                .FirstOrDefault(x => x.Name == CommandsFieldName);
            if (fieldInfo == null)
                return;

            _supportedCommands = (List<(Action action, string name, string description, string assembly)>)fieldInfo.GetValue(null);
        }

        void Update()
        {
            string currentPlaceholderText = _placeholderText.text;

            if (string.IsNullOrEmpty(_commandInputField.text) && string.IsNullOrEmpty(currentPlaceholderText)  && !_placeholderText.isActiveAndEnabled)
            {
                _placeholderText.text = PlaceholderDefaultText;
                _placeholderText.enabled = true;
                return;
            }

            if (!string.IsNullOrEmpty(_commandInputField.text) && !string.IsNullOrEmpty(currentPlaceholderText) && currentPlaceholderText != PlaceholderDefaultText)
            {
                _placeholderText.enabled = true;
            }
        }

        void ToggleConsole(InputAction.CallbackContext callbackContext)
        {
            _placeholderText.text = PlaceholderDefaultText;
            _placeholderText.enabled = true;
            _commandInputField.text = string.Empty;

            if (!callbackContext.action.triggered)
                return;

            if (_debugConsole.activeSelf)
            {
                _debugConsole.SetActive(false);
            }
            else
            {
                _debugConsole.SetActive(true);
                _commandInputField.ActivateInputField();
                UpdatePlaceholderText(PlaceholderDefaultText);
            }
        }

        void CallCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            Debug.Log(command);
            ManageReceivedCommand(command);

            ClearInputField();
        }

        void ManageReceivedCommand(string command)
        {
            if (!_supportedCommands.Any())
            {
                Debug.Log("Supported Commands are empty");
                UpdatePlaceholderText(NoCommandAvailableText);
                return;
            }

            var commandToInvoke = _supportedCommands.FirstOrDefault(x => x.name == command);
            if (commandToInvoke.action == null)
            {
                Debug.Log($"Received command: {command} was not found.");
                return;
            }

            Debug.Log($"Calling command named: {commandToInvoke.name}, {commandToInvoke.description}");
            commandToInvoke.action.Invoke();
        }

        void TakeBestMatchAsCommand()
        {
            _commandInputField.text = _currentBestMatch;
            _commandInputField.caretPosition = _commandInputField.text.Length;
        }

        void GetBestMatch(string partOfCommand)
        {
            string placeholderText;

            if (string.IsNullOrEmpty(partOfCommand))
            {
                _currentBestMatch = string.Empty;
                placeholderText = PlaceholderDefaultText;
                UpdatePlaceholderText(placeholderText);

                return;
            }
            
            var find = _supportedCommands.Where(x => x.name.StartsWith(partOfCommand)).ToList();
            if (find.Count == 1)
            {
                _currentBestMatch = placeholderText = find.First().name;
            }
            else
            {
                var foundCommand = find.FirstOrDefault();
                string currentCommandName = foundCommand.name ?? _supportedCommands.First().name;

                _currentBestMatch = placeholderText = currentCommandName;
            }

            UpdatePlaceholderText(placeholderText);
        }

        void UpdatePlaceholderText(string text)
        {
            _placeholderText.text = text;
        }

        void ClearInputField()
        {
            _commandInputField.text = string.Empty;
            _commandInputField.ActivateInputField();
        }
    }
}
#endif