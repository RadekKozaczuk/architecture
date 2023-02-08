using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shared.CheatEngine;
using TMPro;
using UI.Config;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class DebugMobileConsoleView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField]
        GameObject _scrollContentGameObject;

        List<(Action action, string name, string description, string assembly)> _supportedCommands;
        static readonly UIDebugConfig _config = null!;

        const string ButtonGameObjectName = "Button";
        const string TextGameObjectName = "Text";
        const string CommandsFieldName = "_commands";

        void Awake()
        {
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
            Destroy(gameObject);
        }

        void FixScrollContentSize()
        {
            float debugCommandSizeY = _scrollContentGameObject.GetComponent<GridLayoutGroup>().cellSize.y;

            _scrollContentGameObject.GetComponent<RectTransform>()
                .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, debugCommandSizeY * _supportedCommands.Count);
        }
#endif
    }
}