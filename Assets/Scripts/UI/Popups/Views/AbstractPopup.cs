#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Common.Enums;
using Presentation.ViewModels;
using UnityEngine;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    abstract class AbstractPopup : MonoBehaviour
    {
        internal readonly PopupType Type;

        protected AbstractPopup(PopupType type) => Type = type;

        internal virtual void Initialize()
        {
            RectTransform rect = GetComponent<RectTransform>();

            // if game is on portrait mode change popup anchors
            // leave free 1% of screen on the right and left sides
            if (PresentationViewModel.CurrentScreenOrientation == ScreenOrientation.Portrait)
            {
                rect.anchorMin = new Vector2(0.01f, rect.anchorMin.y);
                rect.anchorMax = new Vector2(0.99f, rect.anchorMax.y);
            }

            SetPopupHightSize(rect);
        }

        internal virtual void Close() { }

        void SetPopupHightSize(RectTransform rect)
        {
            Vector2 currentSize = rect.rect.size;
            float scaleMultiplierY = currentSize.y / currentSize.x;
            float newHightSize = rect.rect.width * scaleMultiplierY;

            Vector2 deltaSize = new Vector2(currentSize.x, newHightSize) - currentSize;

            rect.offsetMin = rect.offsetMin - new Vector2(deltaSize.x * rect.pivot.x, deltaSize.y * rect.pivot.y);
            rect.offsetMax = rect.offsetMax + new Vector2(deltaSize.x * (1f - rect.pivot.x), deltaSize.y * (1f - rect.pivot.y));
        }
    }
}