#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && UNITY_STANDALONE_WIN
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shared.CheatEngine;
using TMPro;
using UI.Config;
using UnityEngine;
using UnityEngine.InputSystem;

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

        static readonly UIConfig _uiConfig = null!;
        const string CommandsFieldName = "_commands";
        const string PlaceholderDefaultText = "Enter command...";
        const string NoCommandAvailableText = "No command available";
        List<(Action action, string name, string description, string assembly)> _supportedCommands;
        string _currentBestMatch;

        void Awake()
        {
            FieldInfo fieldInfo = typeof(DebugCommands).GetFields(BindingFlags.NonPublic | BindingFlags.Static)
                                                       .FirstOrDefault(x => x.Name == CommandsFieldName);
            if (fieldInfo == null)
                return;

            _supportedCommands = (List<(Action action, string name, string description, string assembly)>)fieldInfo.GetValue(null);
        }

        internal void UpdatePlaceholderText()
        {
            string currentPlaceholderText = _placeholderText.text;

            if (string.IsNullOrEmpty(_commandInputField.text) && string.IsNullOrEmpty(currentPlaceholderText) &&
                !_placeholderText.isActiveAndEnabled)
            {
                _placeholderText.text = PlaceholderDefaultText;
                _placeholderText.enabled = true;
                return;
            }

            if (!string.IsNullOrEmpty(_commandInputField.text) && !string.IsNullOrEmpty(currentPlaceholderText) &&
                currentPlaceholderText != PlaceholderDefaultText)
            {
                _placeholderText.enabled = true;
            }
        }

        internal void ToggleConsole(InputAction.CallbackContext callbackContext)
        {
            _placeholderText.text = PlaceholderDefaultText;
            _placeholderText.enabled = true;
            _commandInputField.text = string.Empty;

            if (!callbackContext.action.triggered)
                return;

            if (_debugConsole.activeSelf)
            {
                _debugConsole.SetActive(false);
                _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap).Enable();
            }
            else
            {
                _debugConsole.SetActive(true);
                _commandInputField.ActivateInputField();
                _placeholderText.text = PlaceholderDefaultText;
                _uiConfig.InputActionAsset.FindActionMap(UIConstants.GameplayActionMap).Disable();
            }
        }

        internal void TakeBestMatchAsCommand()
        {
            _commandInputField.text = _currentBestMatch;
            _commandInputField.MoveTextEnd(false); //Fixing caret position
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

            (Action action, string name, string description, string assembly) commandToInvoke = _supportedCommands.FirstOrDefault(x => x.name == command);
            if (commandToInvoke.action == null)
                return;

            commandToInvoke.action.Invoke();
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

            List<(Action action, string name, string description, string assembly)> find = _supportedCommands.Where(x => x.name.StartsWith(partOfCommand)).ToList();
            if (find.Count == 1)
            {
                _currentBestMatch = placeholderText = find.First().name;
            }
            else
            {
                (Action action, string name, string description, string assembly) foundCommand = find.FirstOrDefault();
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
    }
}
#endif