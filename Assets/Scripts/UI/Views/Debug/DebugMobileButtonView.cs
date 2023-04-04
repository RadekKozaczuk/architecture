using System;
using UI.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class DebugMobileButtonView : MonoBehaviour, IPointerClickHandler
    {
        Button _button;

        static readonly UIDebugConfig _uiDebugConfig = null!;

        DateTime _firstClickTime;
        byte _clicks;

        void Awake()
        {
            SetUpButton();
        }

        void SetUpButton()
        {
			_button = this.gameObject.AddComponent<Button>();
			var buttonImage = this.gameObject.AddComponent<Image>();
			buttonImage.color = new Color(0f, 0f, 0f, 0f);
		}

        public void OnPointerClick(PointerEventData eventData)
        {
            double elapsedSeconds = (DateTime.Now - _firstClickTime).TotalSeconds;
            if (elapsedSeconds > _uiDebugConfig.TripleClickDuration)
                _clicks = 0;

            _clicks++;

            if (_clicks == 1)
            {
                _firstClickTime = DateTime.Now;
            }
            else if (_clicks > 2)
            {
                if (elapsedSeconds > _uiDebugConfig.TripleClickDuration)
                    return;

                if (UIData.DebugMobileConsole == null)
                {
                    InstantiateMobileDebugConsole();
                    _button.interactable = false;
                }
            }
        }

        void ChangeButtonInteractables()
        {
            _button.interactable = !_button.interactable;
        }


        static void InstantiateMobileDebugConsole()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            GameObject debugMobileConsole = new GameObject("DebugMobileConsole");
            debugMobileConsole.transform.SetParent(UISceneReferenceHolder.Canvas.transform);

            var rect = debugMobileConsole.AddComponent<RectTransform>();
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);

            debugMobileConsole.AddComponent<DebugMobileConsoleView>();
            UIData.DebugMobileConsole = debugMobileConsole.GetComponent<DebugMobileConsoleView>();
#endif
        }
	}
}