using System;
using UI.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class DebugMobileButtonView : MonoBehaviour, IPointerClickHandler
    {
        static readonly UIDebugConfig _uiDebugConfig = null!;

        [SerializeField]
        internal DebugMobileConsoleView DebugMobileConsole;
        [SerializeField]
        Button _button;

        DateTime _firstClickTime;
        byte _clicks;

        void Start()
        {
            // todo: the reference should be in UI scene reference holder or in a parent view
            // we never look for things
            DebugMobileConsole = FindObjectOfType<DebugMobileConsoleView>();
            DebugMobileConsole.DebugMobileButton = _button;
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

                if (_button.interactable)
                {
                    // todo 
                    // UISceneReferenceHolder.MobileDebugConsoleBackground.SetActive(true);

                    // not this
                    DebugMobileConsole.MobileDebugConsoleBackground.SetActive(true);
                    _button.interactable = false;
                }
            }
        }
    }
}