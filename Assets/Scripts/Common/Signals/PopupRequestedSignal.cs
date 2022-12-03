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