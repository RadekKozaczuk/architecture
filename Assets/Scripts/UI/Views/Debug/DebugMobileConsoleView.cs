using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Config;
using Shared.CheatEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class DebugMobileConsoleView : MonoBehaviour
    {
        static DebugConfig _config = null!;

        [SerializeField]
        internal GameObject MobileDebugConsoleBackground;
        internal Button DebugMobileButton;

        [SerializeField]
        GameObject _scrollContentGameObject;

        List<(Action action, string name, string description, string assembly)> _supportedCommands;

        const string ButtonGameObjectName = "Button";
        const string TextGameObjectName = "Text";
        const string CommandsFieldName = "_commands";

        void Start()
        {
            UIData.DebugMobileConsole = this;

            FieldInfo fieldInfo = typeof(DebugCommands).GetFields(BindingFlags.NonPublic | BindingFlags.Static).FirstOrDefault(x => x.Name == CommandsFieldName);
            if (fieldInfo == null)
                return;

            _supportedCommands = (List<(Action action, string name, string description, string assembly)>)fieldInfo.GetValue(null);
            foreach ((Action action, string name, string description, string assembly) supportedCommand in _supportedCommands)
            {
                GameObject newButton = Instantiate(_config.CommandPrefab, _scrollContentGameObject.transform);

                var button = newButton.transform.Find(ButtonGameObjectName).GetComponent<Button>();

                button.onClick.AddListener(() => supportedCommand.action.Invoke());
                button.transform.Find(TextGameObjectName).GetComponent<TextMeshProUGUI>().text = supportedCommand.name;
            }

            FixScrollContentSize();
        }

        void CloseConsole()
        {
            MobileDebugConsoleBackground.SetActive(false);
            DebugMobileButton.interactable = true;
        }

        void FixScrollContentSize()
        {
            float debugCommandSizeY = _scrollContentGameObject.GetComponent<GridLayoutGroup>().cellSize.y;

            _scrollContentGameObject.GetComponent<RectTransform>()
                .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, debugCommandSizeY * _supportedCommands.Count);
        }
    }
}