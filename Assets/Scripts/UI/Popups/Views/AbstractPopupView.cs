using Common.Enums;
using UnityEngine;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    abstract class AbstractPopupView : MonoBehaviour
    {
        internal readonly PopupType Type;

        protected AbstractPopupView(PopupType type) => Type = type;

        internal abstract void Initialize();

        internal virtual void Close() { }
    }
}