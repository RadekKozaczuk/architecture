#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Common.Enums;
using UnityEngine;

namespace UI.Popups.Views
{
    [DisallowMultipleComponent]
    abstract class AbstractPopup : MonoBehaviour
    {
        internal readonly PopupType Type;

        protected AbstractPopup(PopupType type) => Type = type;

        internal virtual void Initialize() { }

        internal virtual void Close() { }
    }
}