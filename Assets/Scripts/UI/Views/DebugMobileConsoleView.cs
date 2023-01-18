using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common;
using Shared.CheatEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class DebugMobileConsoleView : MonoBehaviour
    {
        [SerializeField]
        GameObject _mobileDebugConsole;
        [SerializeField]
        GameObject _scrollContentGameObject;
        [SerializeField]
        GameObject _commandPrefab;

        List<(Action action, string name, string description, string assembly)> _supportedCommands;

        const string ButtonGameObjectName = "Button";
        const string TextGameObjectName = "Text";
        const string CommandsFieldName = "_commands";

        void Awake()
        {
            CommonDebugCommands.InitializeDebugCommands();

            var fieldInfo = typeof(DebugCommands).GetFields(BindingFlags.NonPublic | BindingFlags.Static).FirstOrDefault(x => x.Name == CommandsFieldName);
            if (fieldInfo == null)
                return;

            _supportedCommands = (List<(Action action, string name, string description, string assembly)>)fieldInfo.GetValue(null);
            foreach (var supportedCommand in _supportedCommands)
            {
                var newButton = Instantiate(_commandPrefab, _scrollContentGameObject.transform);

                var button = newButton.transform.Find(ButtonGameObjectName).GetComponent<Button>();

                button.onClick.AddListener(() => supportedCommand.action.Invoke());
                button.transform.Find(TextGameObjectName).GetComponent<TextMeshProUGUI>().text = supportedCommand.name;
            }

            FixScrollContentSize();
        }

        void CloseConsole()
        {
            _mobileDebugConsole.SetActive(false);
        }

        void FixScrollContentSize()
        {
            float debugCommandSizeY = _scrollContentGameObject.GetComponent<GridLayoutGroup>().cellSize.y;

            _scrollContentGameObject.GetComponent<RectTransform>()
                .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, debugCommandSizeY * _supportedCommands.Count);
        }
    }
}