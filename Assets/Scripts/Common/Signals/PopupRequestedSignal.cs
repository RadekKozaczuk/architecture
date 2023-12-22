#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Common.Enums;
using Shared;

namespace Common.Signals
{
    public sealed class PopupRequestedSignal : AbstractSignal
    {
        public readonly PopupType PopupType;

        public PopupRequestedSignal(PopupType popupType) => PopupType = popupType;
    }
}