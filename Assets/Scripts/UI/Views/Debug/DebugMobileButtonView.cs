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
        [SerializeField]
        internal Button Button;

        static readonly UIDebugConfig _uiDebugConfig = null!;

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

                if (Button.interactable)
                {
                    InstantiateMobileDebugConsole();
                    Button.interactable = false;
                }
            }
        }

        static void InstantiateMobileDebugConsole()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            DebugMobileConsoleView debugMobileConsole = Instantiate(_uiDebugConfig.MobileConsolePrefab, new Vector3(0, 0, -1), Quaternion.identity,
                                                                    UISceneReferenceHolder.Canvas.transform);
            debugMobileConsole.name = "DebugMobileConsole";
            UIData.DebugMobileConsole = debugMobileConsole;
            var rect = debugMobileConsole.GetComponent<RectTransform>();
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, 0);
#endif
        }
    }
}