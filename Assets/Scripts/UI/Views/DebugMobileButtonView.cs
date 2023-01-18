using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Views
{
    [DisallowMultipleComponent]
    class DebugMobileButtonView : MonoBehaviour, IPointerClickHandler
    {
        [Tooltip("Duration between 3 clicks in seconds")]
        [Range(0.01f, 0.5f)]
        public float TripleClickDuration = 0.5f;

        [SerializeField]
        GameObject _debugMobileConsole;
        [SerializeField]
        Button _button;

        DateTime _firstClickTime;
        byte _clicks;

        public void OnPointerClick(PointerEventData eventData)
        {
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && (UNITY_ANDROID || UNITY_IPHONE)
            double elapsedSeconds = (DateTime.Now - _firstClickTime).TotalSeconds;
            if (elapsedSeconds > TripleClickDuration)
                _clicks = 0;

            _clicks++;

            if (_clicks == 1)
            {
                _firstClickTime = DateTime.Now;
            }
            else if (_clicks > 2)
            {
                if (!(elapsedSeconds <= TripleClickDuration))
                {
                    return;
                }

                if (_button.interactable)
                {
                    _debugMobileConsole.SetActive(true);
                }
            }
#endif
        }
    }
}