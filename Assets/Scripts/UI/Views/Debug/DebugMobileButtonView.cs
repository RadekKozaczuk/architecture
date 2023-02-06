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
        static readonly UIDebugConfig _uiDebugConfig = null!;

        [SerializeField]
        Button _button;

        DateTime _firstClickTime;
        byte _clicks;

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

                if (_button.interactable)
                {
                    InstantiateMobileDebugConsole();
                    _button.interactable = false;
                }
            }
        }

        void InstantiateMobileDebugConsole()
        {
            DebugMobileConsoleView debugMobileConsole = Instantiate(_uiDebugConfig.MobileConsolePrefab, new Vector3(0, 0, -1),
                Quaternion.identity, UISceneReferenceHolder.Canvas.transform);
            debugMobileConsole.name = "DebugMobileConsole";
            var rectMobileConsoleComponent = debugMobileConsole.GetComponent<RectTransform>(); //rename
            rectMobileConsoleComponent.offsetMin = new Vector2(0, 0);
            rectMobileConsoleComponent.offsetMax = new Vector2(0, 0);

            UIData.DebugMobileConsole.DebugMobileButton = _button;
        }
    }
}