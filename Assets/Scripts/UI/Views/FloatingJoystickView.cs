#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting
using System.Collections;
using System.Collections.Generic;
using UI.Systems;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Views
{
    class FloatingJoystickView : JoystickSystem
    {
        protected override void Start()
        {
            base.Start();
            background.gameObject.SetActive(false);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            background.gameObject.SetActive(true);
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            background.gameObject.SetActive(false);
            base.OnPointerUp(eventData);
        }
    }
}