using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shared.CheatEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class DebugConsoleView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        TMP_InputField _commandInputField;
        TMP_Text _placeholderText;
        RectTransform _commandsHistoryHolder;

        const string CommandsFieldName = "_commands";
        const string PlaceholderDefaultText = "Enter command...";
        const string NoCommandAvailableText = "No command available";
        List<string> _commandsHistory = new List<string>();
        List<(Action<int> action, string name, bool parameters, string description, string assembly)> _supportedCommands;
        string _currentBestMatch;
        private Color _backgroundColor = new Color(0f, 0f, 0f, 0.42f);
        private bool _showingHistory = false;

		private void Awake()
        {
            SpawnConsole();
		}

        void SpawnConsole()
        {
            RectTransform rectTransform = this.gameObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(Screen.width - 50f, 50);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);

            //Background Image Setup
            Image backgroundImage = new GameObject("DebugConsoleBackground").AddComponent<Image>();
            backgroundImage.transform.SetParent(this.transform);
            RectTransform imageRect = backgroundImage.gameObject.GetComponent<RectTransform>();
            SetRectTransform(imageRect);
            backgroundImage.color = _backgroundColor;

            TextMeshProUGUI placeholder = new GameObject("Placeholder").AddComponent<TextMeshProUGUI>();
			TextMeshProUGUI inputText = new GameObject("Input text").AddComponent<TextMeshProUGUI>();
            RectMask2D TextArea = new GameObject("Text Area").AddComponent<RectMask2D>();

            //Input Field Setup
            GameObject inputFieldGO = new GameObject("Input field");
            inputFieldGO.transform.SetParent(backgroundImage.transform);
            inputFieldGO.AddComponent<RectTransform>();
            inputFieldGO.AddComponent<TMP_InputField>();
            _commandInputField = inputFieldGO.GetComponent<TMP_InputField>();
            _commandInputField.textViewport = TextArea.GetComponent<RectTransform>();
            _commandInputField.textComponent = inputText.GetComponent<TextMeshProUGUI>();
            _commandInputField.placeholder = placeholder.GetComponent<TextMeshProUGUI>();
            _commandInputField.onValueChanged.AddListener(GetBestMatch);
            _commandInputField.onEndEdit.AddListener(CallCommand);

            SetRectTransform(inputFieldGO.GetComponent<RectTransform>());

            placeholder.transform.SetParent(TextArea.transform);
            TextArea.transform.SetParent(inputFieldGO.transform);
            SetRectTransform(TextArea.GetComponent<RectTransform>());
			inputText.transform.SetParent(TextArea.transform);
            SetRectTransform(inputText.GetComponent<RectTransform>());
            _placeholderText = placeholder;
            _placeholderText.color = new Color(0f, 0f, 0.05f, 0.5f);
            _placeholderText.text = PlaceholderDefaultText;
            SetRectTransform(placeholder.GetComponent<RectTransform>());

            //Show history button setup
            Button showMoreButton = new GameObject("Show More").AddComponent<Button>();
            showMoreButton.transform.SetParent(backgroundImage.transform);
            Image buttonImage = showMoreButton.gameObject.AddComponent<Image>();
            buttonImage.color = Color.black;
            showMoreButton.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 25f);
            showMoreButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -25f);
            showMoreButton.onClick.AddListener(ShowCommandsHistory);
		}

		private void SetRectTransform(RectTransform rt)
        {
			rt.anchorMax = new Vector2(1f, 1f);
			rt.anchorMin = new Vector2(0f, 0f);
			rt.pivot = new Vector2(0.5f, 0.5f);
			rt.sizeDelta = new Vector2(0f, 0f);
			rt.offsetMax = new Vector2(0f, 0f);
			rt.offsetMin = new Vector2(0f, 0f);
		}

        private void ShowCommandsHistory()
        {
            if (_showingHistory) {
                Destroy(_commandsHistoryHolder.gameObject);
                _showingHistory = false;
                return;
            }
            _showingHistory = true;
            int commandsCount = _commandsHistory.Count <= 5 ? _commandsHistory.Count : 5;

            _commandsHistoryHolder = new GameObject("Commands history holder").AddComponent<RectTransform>();
            _commandsHistoryHolder.transform.SetParent(this.transform);
            _commandsHistoryHolder.transform.SetAsFirstSibling();
            SetRectTransform(_commandsHistoryHolder);
            Image commandsHistoryBackground = new GameObject("HistoryBackground").AddComponent<Image>();
            commandsHistoryBackground.color = _backgroundColor;
            commandsHistoryBackground.transform.SetParent(_commandsHistoryHolder.transform);
            commandsHistoryBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width - 50f, 50f * commandsCount);
			_commandsHistoryHolder.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -25f*(commandsCount+1));
            commandsHistoryBackground.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
			for (int i = 0; i < commandsCount; i++) {
                TextMeshProUGUI commandHistory = new GameObject("history").AddComponent<TextMeshProUGUI>();
                commandHistory.transform.SetParent(_commandsHistoryHolder.transform);
                RectTransform commandHistoryRect = commandHistory.GetComponent<RectTransform>();
                commandHistoryRect.sizeDelta = new Vector2(Screen.width - 50f, 50f);
                commandHistoryRect.anchoredPosition = new Vector2(0f, commandsCount * 25f - 50f * i - 25f);
                commandHistory.text = _commandsHistory[commandsCount - i - 1];
                commandHistory.alignment = TextAlignmentOptions.CaplineLeft;
            }
        }

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
            _commandsHistory.Add(command);
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