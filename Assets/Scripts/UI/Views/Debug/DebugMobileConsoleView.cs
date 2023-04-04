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
        GameObject _scrollContentGameObject;

        List<(Action<int> action, string name, bool parameters, string description, string assembly)> _supportedCommands;

        const string CommandsFieldName = "_commands";
        int _defaultValue = 5;

        void Awake()
        {
            InstantiateConsole();

            FieldInfo fieldInfo = typeof(DebugCommands).GetFields(BindingFlags.NonPublic | BindingFlags.Static).FirstOrDefault(x => x.Name == CommandsFieldName);
            if (fieldInfo == null)
                return;

            _supportedCommands = (List<(Action<int> action, string name, bool parameters, string description, string assembly)>)fieldInfo.GetValue(null);
            foreach ((Action<int> action, string name, bool parameters, string description, string assembly) supportedCommand in _supportedCommands)
            {
                GameObject newButton = new GameObject("Command Button");
                newButton.transform.parent = _scrollContentGameObject.transform;
				var button = newButton.AddComponent<Button>();
                newButton.AddComponent<Image>();

                GameObject buttonText = new GameObject("ButtonText");
                buttonText.transform.parent = newButton.transform;
                var buttonTextComponent = buttonText.AddComponent<TextMeshProUGUI>();
                buttonTextComponent.alignment = TextAlignmentOptions.Center;
				string[] commandWords = supportedCommand.name.Split("_");
				string commandName = "";
				foreach (string word in commandWords)
				{
					commandName += word + " ";
				}
				buttonTextComponent.text = commandName;//
                buttonTextComponent.color = Color.black;

                button.onClick.AddListener(() => supportedCommand.action?.Invoke(_defaultValue));
            }

            FixScrollContentSize();
        }

        void InstantiateConsole()
		{
            //BackGround
			var imageComponent = gameObject.AddComponent<Image>();
			imageComponent.rectTransform.anchorMin = Vector2.zero;
			imageComponent.rectTransform.anchorMax = Vector2.one;
			imageComponent.rectTransform.anchoredPosition = Vector2.zero;
			imageComponent.rectTransform.sizeDelta = Vector2.zero;
			imageComponent.color = new Color(0f, 0f, 255f, 0.4f);

			//Close Button
			GameObject closeButtonGo = new GameObject("Close Button");
			closeButtonGo.transform.SetParent(transform);

			Button closeButtonComponent = closeButtonGo.AddComponent<Button>();
			var closeButtonImage = closeButtonGo.AddComponent<Image>();

			var closeButtonRect = closeButtonGo.GetComponent<RectTransform>();
			closeButtonRect.anchorMin = new Vector2(0f, 1f);
			closeButtonRect.anchorMax = new Vector2(0f, 1f);
			closeButtonRect.pivot = new Vector2(0.5f, 0.5f);
			closeButtonRect.anchoredPosition = new Vector2(25f, -25f);

			GameObject closeButtonTextGo = new GameObject("Text");
			closeButtonTextGo.transform.SetParent(closeButtonGo.transform);
			var closeButtonText = closeButtonTextGo.AddComponent<TextMeshProUGUI>();

			var closeButtonTextRect = closeButtonTextGo.GetComponent<RectTransform>();
			closeButtonTextRect.anchorMin = new Vector2(0f, 1f);
			closeButtonTextRect.anchorMax = new Vector2(0f, 1f);
			closeButtonTextRect.pivot = new Vector2(0.5f, 0.5f);
			closeButtonTextRect.anchoredPosition = new Vector2(25f, -25f);

			closeButtonText.text = "X";
			closeButtonText.fontSize = 40;
			closeButtonText.alignment = TextAlignmentOptions.Center;

			closeButtonImage.color = Color.black;
            Vector2 buttonSize = new Vector2(50f, 50f);
			closeButtonRect.sizeDelta = buttonSize;

            closeButtonComponent.onClick.AddListener(CloseConsole);

            //Scroll Content
            GameObject scrollContentGo = new GameObject("Scroll Content");
            scrollContentGo.transform.SetParent(transform);
            var scrollRectComponent = scrollContentGo.AddComponent<ScrollRect>();
            var scrollRect = scrollContentGo.GetComponent<RectTransform>();
            scrollRect.anchorMin = Vector2.zero;
			scrollRect.anchorMax = Vector2.one;
			scrollRect.offsetMax = new Vector2(0, -200);
			scrollRect.offsetMin = new Vector2(0, 0);

            _scrollContentGameObject = new GameObject("Content");
            _scrollContentGameObject.transform.SetParent(scrollContentGo.transform);
            var contentGridLayout = _scrollContentGameObject.AddComponent<GridLayoutGroup>();
            float cellWidth = 400f;
            float cellHeight = 100f;
            contentGridLayout.cellSize = new Vector2(cellWidth, cellHeight);
            float spacingX = 20f;
            float spacingY = 20f;
            contentGridLayout.spacing = new Vector2(spacingX, spacingY);
            var scrollContentRect = _scrollContentGameObject.GetComponent<RectTransform>();
			scrollContentRect.anchorMin = Vector2.zero;
			scrollContentRect.anchorMax = Vector2.one;
			scrollContentRect.offsetMax = Vector2.zero;
			scrollContentRect.offsetMin = Vector2.zero;

			scrollRectComponent.content = _scrollContentGameObject.GetComponent<RectTransform>();
		}

		void CloseConsole()
        {
            UIData.DebugMobileConsole = null;
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